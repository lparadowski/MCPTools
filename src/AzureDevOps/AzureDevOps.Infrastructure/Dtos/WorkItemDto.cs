using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class WorkItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("fields")]
    public WorkItemFieldsDto? Fields { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
