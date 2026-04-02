using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class WiqlResultDto
{
    [JsonPropertyName("workItems")]
    public List<WiqlWorkItemRefDto> WorkItems { get; set; } = [];
}
