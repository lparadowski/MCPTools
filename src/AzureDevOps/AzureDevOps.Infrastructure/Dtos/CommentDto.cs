using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class CommentDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("createdBy")]
    public IdentityRefDto? CreatedBy { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime? CreatedDate { get; set; }
}
