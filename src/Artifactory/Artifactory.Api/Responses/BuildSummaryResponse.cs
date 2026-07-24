namespace Artifactory.Api.Responses;

public class BuildSummaryResponse
{
    public required string Name { get; set; }
    public string? LastStarted { get; set; }
}
