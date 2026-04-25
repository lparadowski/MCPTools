using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class WorkItemFieldsDto
{
    [JsonPropertyName("System.Title")]
    public string? Title { get; set; }

    [JsonPropertyName("System.Description")]
    public string? Description { get; set; }

    [JsonPropertyName("System.WorkItemType")]
    public string? WorkItemType { get; set; }

    [JsonPropertyName("System.State")]
    public string? State { get; set; }

    [JsonPropertyName("System.AssignedTo")]
    public IdentityRefDto? AssignedTo { get; set; }

    [JsonPropertyName("System.AreaPath")]
    public string? AreaPath { get; set; }

    [JsonPropertyName("System.IterationPath")]
    public string? IterationPath { get; set; }

    [JsonPropertyName("Microsoft.VSTS.Common.Priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("System.Tags")]
    public string? Tags { get; set; }

    [JsonPropertyName("System.Parent")]
    public int? Parent { get; set; }

    [JsonPropertyName("System.TeamProject")]
    public string? TeamProject { get; set; }

    [JsonPropertyName("System.CreatedDate")]
    public DateTime? CreatedDate { get; set; }

    [JsonPropertyName("System.ChangedDate")]
    public DateTime? ChangedDate { get; set; }
}
