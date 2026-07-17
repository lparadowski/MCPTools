namespace Octopus.Domain.Entities;

public class DeployedRelease
{
    public required string ProjectId { get; set; }
    public required string ProjectName { get; set; }
    public required string EnvironmentId { get; set; }
    public required string EnvironmentName { get; set; }
    public required string ReleaseId { get; set; }
    public required string Version { get; set; }
    public string DeploymentId { get; set; } = string.Empty;
    public DateTimeOffset DeployedAt { get; set; }
    public string? State { get; set; }
}
