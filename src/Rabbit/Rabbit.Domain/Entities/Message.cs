namespace Rabbit.Domain.Entities;

public class Message
{
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public required bool Redelivered { get; set; }
    public required ulong DeliveryTag { get; set; }
    public string? BodyText { get; set; }
    public string? BodyBase64 { get; set; }
    public Dictionary<string, string?>? Headers { get; set; }
    public string? ContentType { get; set; }
    public string? ContentEncoding { get; set; }
    public string? MessageId { get; set; }
    public string? CorrelationId { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}
