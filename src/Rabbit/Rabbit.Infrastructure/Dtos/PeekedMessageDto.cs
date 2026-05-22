using System.Text.Json.Serialization;

namespace Rabbit.Infrastructure.Dtos;

internal sealed class PeekedMessageDto
{
    public string? Exchange { get; set; }

    [JsonPropertyName("routing_key")]
    public string? RoutingKey { get; set; }

    public bool Redelivered { get; set; }

    public string? Payload { get; set; }

    [JsonPropertyName("payload_encoding")]
    public string? PayloadEncoding { get; set; }

    public PeekedMessagePropertiesDto? Properties { get; set; }
}

internal sealed class PeekedMessagePropertiesDto
{
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("content_encoding")]
    public string? ContentEncoding { get; set; }

    [JsonPropertyName("message_id")]
    public string? MessageId { get; set; }

    [JsonPropertyName("correlation_id")]
    public string? CorrelationId { get; set; }

    public object? Timestamp { get; set; }

    public Dictionary<string, object?>? Headers { get; set; }
}
