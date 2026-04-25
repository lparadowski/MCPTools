using System.Text.Json.Serialization;

namespace Polarion.Infrastructure.Dtos;

public class PolarionSingleResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
