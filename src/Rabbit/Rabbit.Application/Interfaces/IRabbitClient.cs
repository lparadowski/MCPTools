using Rabbit.Domain.Entities;

namespace Rabbit.Application.Interfaces;

public interface IRabbitClient
{
    // Subscriptions (AMQP)
    Task<Subscription> SubscribeAsync(string exchange, string routingKey, string? queueName, CancellationToken cancellationToken = default);
    Task<List<Message>> DrainAsync(string subscriptionId, int maxMessages, int waitMs, CancellationToken cancellationToken = default);
    Task<bool> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
    IReadOnlyList<Subscription> ListSubscriptions();

    // Publishing (AMQP)
    Task PublishAsync(string exchange, string routingKey, string body, string? contentType, Dictionary<string, string>? headers, CancellationToken cancellationToken = default);

    // Broker introspection (Management API)
    Task<List<Exchange>> ListExchangesAsync(CancellationToken cancellationToken = default);
    Task<List<Queue>> ListQueuesAsync(CancellationToken cancellationToken = default);
    Task<List<Message>> PeekAsync(string queueName, int count, CancellationToken cancellationToken = default);
}
