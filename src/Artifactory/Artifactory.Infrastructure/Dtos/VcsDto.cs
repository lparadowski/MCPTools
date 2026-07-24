namespace Artifactory.Infrastructure.Dtos;

internal class VcsDto
{
    public string? Revision { get; set; }
    public string? Branch { get; set; }
    public string? Url { get; set; }
    public string? Message { get; set; }
}
