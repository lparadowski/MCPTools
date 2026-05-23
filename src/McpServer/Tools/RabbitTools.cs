using System.ComponentModel;
using System.Net.Http.Json;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class RabbitTools
{
    // Subscriptions

    [McpServerTool(Name = "rabbit_list_subscriptions")]
    [Description("List active RabbitMQ subscriptions (transient queues + bindings created via rabbit_subscribe).")]
    public static async Task<string> ListSubscriptions(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.GetAsync("/api/v1/subscriptions");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "rabbit_subscribe")]
    [Description("Create a transient queue and bind it to the given exchange + routing key so messages can be drained later. Returns a subscription id. Queue name is server-generated if not provided.")]
    public static async Task<string> Subscribe(
        IHttpClientFactory httpFactory,
        [Description("Exchange name to bind to (must already exist).")] string exchange,
        [Description("Routing key pattern (e.g. 'asset.AHT001.eta' or '#').")] string routingKey,
        [Description("Optional queue name. Leave empty for a server-generated transient name.")] string? queueName = null)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.PostAsJsonAsync("/api/v1/subscriptions", new { exchange, routingKey, queueName });
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "rabbit_drain")]
    [Description("Consume up to maxMessages from a subscription's queue, waiting up to waitMs for at least one. Messages are acked. Returns the batch with body, routing key, headers, etc.")]
    public static async Task<string> Drain(
        IHttpClientFactory httpFactory,
        [Description("Subscription id returned by rabbit_subscribe.")] string subscriptionId,
        [Description("Maximum number of messages to return (default 10).")] int maxMessages = 10,
        [Description("Maximum milliseconds to wait for the first message (default 2000).")] int waitMs = 2000)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.PostAsync(
            $"/api/v1/subscriptions/{subscriptionId}/drain?maxMessages={maxMessages}&waitMs={waitMs}",
            content: null);
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "rabbit_unsubscribe")]
    [Description("Delete a subscription's queue + binding. Always call this when done to keep the broker clean.")]
    public static async Task<string> Unsubscribe(
        IHttpClientFactory httpFactory,
        [Description("Subscription id returned by rabbit_subscribe.")] string subscriptionId)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.DeleteAsync($"/api/v1/subscriptions/{subscriptionId}");
        return await response.ReadContentOrError();
    }

    // Publishing

    [McpServerTool(Name = "rabbit_publish")]
    [Description("Publish a single message to an exchange + routing key. To publish directly to a queue, pass exchange=\"\" and the queue name as routingKey (RabbitMQ's default exchange convention). Body is sent as UTF-8.")]
    public static async Task<string> Publish(
        IHttpClientFactory httpFactory,
        [Description("Exchange to publish to. Use empty string \"\" with the queue name as routingKey to publish directly to a queue.")] string exchange,
        [Description("Routing key. For the default exchange, this is the destination queue name.")] string routingKey,
        [Description("Message body (sent as UTF-8 bytes). Typically a JSON string for Imperium events.")] string body,
        [Description("Optional content type, e.g. 'application/json'.")] string? contentType = null,
        [Description("Optional headers (string→string map).")] Dictionary<string, string>? headers = null)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.PostAsJsonAsync("/api/v1/messages", new { exchange, routingKey, body, contentType, headers });
        return await response.ReadContentOrError();
    }

    // Broker introspection

    [McpServerTool(Name = "rabbit_list_exchanges")]
    [Description("List all exchanges on the broker (via management API). Useful when you don't know which exchange to bind to.")]
    public static async Task<string> ListExchanges(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.GetAsync("/api/v1/exchanges");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "rabbit_list_queues")]
    [Description("List all queues on the broker (via management API). Shows queue depth, consumer count, durability, etc.")]
    public static async Task<string> ListQueues(IHttpClientFactory httpFactory)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.GetAsync("/api/v1/queues");
        return await response.ReadContentOrError();
    }

    [McpServerTool(Name = "rabbit_peek")]
    [Description("Non-destructively read up to N messages from an existing queue (via management API ack_requeue_true). Messages are put back on the queue so other consumers still see them.")]
    public static async Task<string> Peek(
        IHttpClientFactory httpFactory,
        [Description("Queue name to peek.")] string queueName,
        [Description("Max messages to fetch (default 5).")] int count = 5)
    {
        var http = httpFactory.CreateClient("RabbitApi");
        var response = await http.PostAsync($"/api/v1/queues/{Uri.EscapeDataString(queueName)}/peek?count={count}", content: null);
        return await response.ReadContentOrError();
    }
}
