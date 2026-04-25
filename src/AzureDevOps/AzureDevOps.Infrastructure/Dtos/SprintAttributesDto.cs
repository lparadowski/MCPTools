using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class SprintAttributesDto
{
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("finishDate")]
    public DateTime? FinishDate { get; set; }

    [JsonPropertyName("timeFrame")]
    public string? TimeFrame { get; set; }
}
