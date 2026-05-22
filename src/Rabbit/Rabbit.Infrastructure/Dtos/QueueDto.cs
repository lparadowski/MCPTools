using System.Text.Json.Serialization;

namespace Rabbit.Infrastructure.Dtos;

internal sealed class QueueDto
{
    public string? Name { get; set; }
    public string? Vhost { get; set; }
    public long Messages { get; set; }

    [JsonPropertyName("messages_ready")]
    public long MessagesReady { get; set; }

    [JsonPropertyName("messages_unacknowledged")]
    public long MessagesUnacknowledged { get; set; }

    public long Consumers { get; set; }
    public string? State { get; set; }
    public bool Durable { get; set; }

    [JsonPropertyName("auto_delete")]
    public bool AutoDelete { get; set; }

    public bool Exclusive { get; set; }
}
