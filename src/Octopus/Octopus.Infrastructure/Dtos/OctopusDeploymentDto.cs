namespace Octopus.Infrastructure.Dtos;

internal class OctopusDeploymentDto
{
    public string Id { get; set; } = string.Empty;
    public string ReleaseId { get; set; } = string.Empty;
    public string EnvironmentId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string? TaskId { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset Created { get; set; }
}
