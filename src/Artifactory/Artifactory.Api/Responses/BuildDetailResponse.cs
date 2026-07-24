namespace Artifactory.Api.Responses;

public class BuildDetailResponse
{
    public required string Name { get; set; }
    public required string Number { get; set; }
    public string? Started { get; set; }
    public string? BuildUrl { get; set; }
    public string? Agent { get; set; }
    public string? CommitSha { get; set; }
    public string? Branch { get; set; }
    public string? RepoUrl { get; set; }
    public List<string> Artifacts { get; set; } = [];
}
