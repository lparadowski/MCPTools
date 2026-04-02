using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class TeamDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("projectName")]
    public string? ProjectName { get; set; }
}
