namespace Rabbit.Api.Requests;

public class PublishMessageRequest
{
    /// <summary>
    /// Exchange to publish to. Use "" (empty string) with the queue name as routingKey to publish directly to a queue via the default exchange.
    /// </summary>
    public required string Exchange { get; set; }

    /// <summary>
    /// Routing key. For the default exchange ("") this is the destination queue name.
    /// </summary>
    public required string RoutingKey { get; set; }

    /// <summary>
    /// Message body as a string (sent as UTF-8 bytes).
    /// </summary>
    public required string Body { get; set; }

    public string? ContentType { get; set; }

    public Dictionary<string, string>? Headers { get; set; }
}
