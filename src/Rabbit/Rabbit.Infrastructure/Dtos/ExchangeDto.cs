using System.Text.Json.Serialization;

namespace Rabbit.Infrastructure.Dtos;

internal sealed class ExchangeDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Vhost { get; set; }
    public bool Durable { get; set; }

    [JsonPropertyName("auto_delete")]
    public bool AutoDelete { get; set; }

    public bool Internal { get; set; }
}
