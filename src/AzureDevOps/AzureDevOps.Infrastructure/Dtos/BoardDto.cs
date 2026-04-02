using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class BoardDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
