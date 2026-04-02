using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class IdentityRefDto
{
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("uniqueName")]
    public string? UniqueName { get; set; }
}
