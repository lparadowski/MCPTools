using System.Text.Json.Serialization;

namespace AzureDevOps.Infrastructure.Dtos;

public class CommentListDto
{
    [JsonPropertyName("comments")]
    public List<CommentDto> Comments { get; set; } = [];
}
