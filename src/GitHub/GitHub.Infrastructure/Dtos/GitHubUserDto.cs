using System.Text.Json.Serialization;

namespace GitHub.Infrastructure.Dtos;

internal class GitHubUserDto
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;
}
