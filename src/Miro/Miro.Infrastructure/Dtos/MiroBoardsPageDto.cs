using System.Text.Json.Serialization;

namespace Miro.Infrastructure.Dtos;

internal class MiroBoardsPageDto
{
    [JsonPropertyName("data")]
    public List<MiroBoardDto>? Data { get; set; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }
}
