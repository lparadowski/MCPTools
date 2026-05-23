using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Rabbit.Application.Interfaces;
using Rabbit.Domain.Entities;
using Rabbit.Infrastructure.Dtos;
using Rabbit.Infrastructure.Settings;
using RabbitMQ.Client;

namespace Rabbit.Infrastructure.Clients;

/// <summary>
/// Single-process façade over RabbitMQ:
///   - AMQP for subscribe/drain/unsubscribe (long-lived connection, one channel per subscription).
///   - Management HTTP API for list-exchanges/list-queues/peek.
/// Subscriptions live in memory; restarting the wrapper loses them. By design for a dev tool.
/// </summary>
public sealed class RabbitClient : IRabbitClient, IAsyncDisposable
{
    private readonly InfrastructureSettings _settings;
    private readonly IHttpClientFactory _httpFactory;
    private readonly ConcurrentDictionary<string, SubscriptionEntry> _subscriptions = new();

    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RabbitClient(InfrastructureSettings settings, IHttpClientFactory httpFactory)
    {
        _settings = settings;
        _httpFactory = httpFactory;
    }

    // ---------- AMQP: subscribe / drain / unsubscribe ----------

    public async Task<Subscription> SubscribeAsync(string exchange, string routingKey, string? queueName, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        // Server-generated queue if no name provided.
        var declared = await channel.QueueDeclareAsync(
            queue: queueName ?? string.Empty,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: declared.QueueName,
            exchange: exchange,
            routingKey: routingKey,
            arguments: null,
            cancellationToken: cancellationToken);

        var subscription = new Subscription
        {
            Id = Guid.NewGuid().ToString("N"),
            Exchange = exchange,
            RoutingKey = routingKey,
            QueueName = declared.QueueName,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _subscriptions[subscription.Id] = new SubscriptionEntry(subscription, channel);
        return subscription;
    }

    public async Task<List<Message>> DrainAsync(string subscriptionId, int maxMessages, int waitMs, CancellationToken cancellationToken = default)
    {
        if (!_subscriptions.TryGetValue(subscriptionId, out var entry))
        {
            throw new KeyNotFoundException($"Subscription '{subscriptionId}' not found.");
        }

        var messages = new List<Message>();
        var deadline = DateTimeOffset.UtcNow.AddMilliseconds(waitMs);

        while (messages.Count < maxMessages && !cancellationToken.IsCancellationRequested)
        {
            var result = await entry.Channel.BasicGetAsync(entry.Subscription.QueueName, autoAck: false, cancellationToken: cancellationToken);
            if (result is null)
            {
                if (messages.Count > 0 || DateTimeOffset.UtcNow >= deadline)
                {
                    break;
                }
                await Task.Delay(100, cancellationToken);
                continue;
            }

            messages.Add(ToMessage(result));
            await entry.Channel.BasicAckAsync(result.DeliveryTag, multiple: false, cancellationToken: cancellationToken);
        }

        return messages;
    }

    public async Task<bool> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        if (!_subscriptions.TryRemove(subscriptionId, out var entry))
        {
            return false;
        }

        try
        {
            await entry.Channel.QueueDeleteAsync(entry.Subscription.QueueName, ifUnused: false, ifEmpty: false, cancellationToken: cancellationToken);
        }
        finally
        {
            await entry.Channel.CloseAsync(cancellationToken);
            await entry.Channel.DisposeAsync();
        }
        return true;
    }

    public IReadOnlyList<Subscription> ListSubscriptions() =>
        _subscriptions.Values.Select(e => e.Subscription).ToList();

    // ---------- AMQP: publish ----------

    public async Task PublishAsync(string exchange, string routingKey, string body, string? contentType, Dictionary<string, string>? headers, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var properties = new BasicProperties();
        if (!string.IsNullOrEmpty(contentType))
        {
            properties.ContentType = contentType;
        }
        if (headers is { Count: > 0 })
        {
            properties.Headers = headers.ToDictionary(kv => kv.Key, kv => (object?)Encoding.UTF8.GetBytes(kv.Value));
        }

        var bodyBytes = Encoding.UTF8.GetBytes(body);
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: bodyBytes,
            cancellationToken: cancellationToken);
    }

    // ---------- Management API: list-exchanges / list-queues / peek ----------

    public async Task<List<Exchange>> ListExchangesAsync(CancellationToken cancellationToken = default)
    {
        var http = CreateManagementClient();
        var dtos = await http.GetFromJsonAsync<List<ExchangeDto>>("/api/exchanges", JsonOptions, cancellationToken) ?? [];
        return dtos.Where(d => !string.IsNullOrEmpty(d.Name)).Select(d => new Exchange
        {
            Name = d.Name!,
            Type = d.Type ?? "direct",
            VirtualHost = d.Vhost ?? "/",
            Durable = d.Durable,
            AutoDelete = d.AutoDelete,
            Internal = d.Internal
        }).ToList();
    }

    public async Task<List<Queue>> ListQueuesAsync(CancellationToken cancellationToken = default)
    {
        var http = CreateManagementClient();
        var dtos = await http.GetFromJsonAsync<List<QueueDto>>("/api/queues", JsonOptions, cancellationToken) ?? [];
        return dtos.Select(d => new Queue
        {
            Name = d.Name ?? string.Empty,
            VirtualHost = d.Vhost ?? "/",
            Messages = d.Messages,
            MessagesReady = d.MessagesReady,
            MessagesUnacknowledged = d.MessagesUnacknowledged,
            Consumers = d.Consumers,
            State = d.State,
            Durable = d.Durable,
            AutoDelete = d.AutoDelete,
            Exclusive = d.Exclusive
        }).ToList();
    }

    public async Task<List<Message>> PeekAsync(string queueName, int count, CancellationToken cancellationToken = default)
    {
        var http = CreateManagementClient();
        var vhost = Uri.EscapeDataString(_settings.Vhost);
        var queue = Uri.EscapeDataString(queueName);

        var request = new
        {
            count,
            ackmode = "ack_requeue_true", // non-destructive: messages put back on the queue
            encoding = "auto",
            truncate = 50_000
        };

        var response = await http.PostAsJsonAsync($"/api/queues/{vhost}/{queue}/get", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var dtos = await response.Content.ReadFromJsonAsync<List<PeekedMessageDto>>(JsonOptions, cancellationToken) ?? [];

        return dtos.Select(d => new Message
        {
            Exchange = d.Exchange ?? string.Empty,
            RoutingKey = d.RoutingKey ?? string.Empty,
            Redelivered = d.Redelivered,
            DeliveryTag = 0,
            BodyText = d.PayloadEncoding == "string" ? d.Payload : null,
            BodyBase64 = d.PayloadEncoding == "base64" ? d.Payload : null,
            Headers = d.Properties?.Headers?.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString()),
            ContentType = d.Properties?.ContentType,
            ContentEncoding = d.Properties?.ContentEncoding,
            MessageId = d.Properties?.MessageId,
            CorrelationId = d.Properties?.CorrelationId,
            Timestamp = d.Properties?.Timestamp is long ts ? DateTimeOffset.FromUnixTimeSeconds(ts) : null
        }).ToList();
    }

    // ---------- Connection / channel management ----------

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.RabbitMqUri),
                ClientProvidedName = "McpProjectTools.Rabbit.Api"
            };
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private HttpClient CreateManagementClient()
    {
        var http = _httpFactory.CreateClient("RabbitManagementApi");
        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_settings.RabbitMqManagementUsername}:{_settings.RabbitMqManagementPassword}"));
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return http;
    }

    private static Message ToMessage(BasicGetResult result)
    {
        var bodyBytes = result.Body.ToArray();
        var isUtf8 = TryDecodeUtf8(bodyBytes, out var text);

        return new Message
        {
            Exchange = result.Exchange,
            RoutingKey = result.RoutingKey,
            Redelivered = result.Redelivered,
            DeliveryTag = result.DeliveryTag,
            BodyText = isUtf8 ? text : null,
            BodyBase64 = isUtf8 ? null : Convert.ToBase64String(bodyBytes),
            Headers = result.BasicProperties.Headers?.ToDictionary(
                kv => kv.Key,
                kv => kv.Value is byte[] b ? Encoding.UTF8.GetString(b) : kv.Value?.ToString()),
            ContentType = result.BasicProperties.ContentType,
            ContentEncoding = result.BasicProperties.ContentEncoding,
            MessageId = result.BasicProperties.MessageId,
            CorrelationId = result.BasicProperties.CorrelationId,
            Timestamp = result.BasicProperties.Timestamp.UnixTime == 0
                ? null
                : DateTimeOffset.FromUnixTimeSeconds(result.BasicProperties.Timestamp.UnixTime)
        };
    }

    private static bool TryDecodeUtf8(byte[] bytes, out string text)
    {
        try
        {
            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
            text = encoding.GetString(bytes);
            return true;
        }
        catch
        {
            text = string.Empty;
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var (_, entry) in _subscriptions)
        {
            try { await entry.Channel.CloseAsync(); } catch { /* swallow */ }
            await entry.Channel.DisposeAsync();
        }
        _subscriptions.Clear();
        if (_connection is not null)
        {
            try { await _connection.CloseAsync(); } catch { /* swallow */ }
            await _connection.DisposeAsync();
        }
    }

    private sealed record SubscriptionEntry(Subscription Subscription, IChannel Channel);
}
