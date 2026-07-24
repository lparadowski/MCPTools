namespace Artifactory.Infrastructure.Dtos;

internal class BuildInfoBodyDto
{
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Started { get; set; }
    public string? Url { get; set; }
    public AgentDto? Agent { get; set; }
    public List<VcsDto>? Vcs { get; set; }
    public List<ModuleDto>? Modules { get; set; }

    // build-info properties are a flat string->string map (unlike storage item properties).
    public Dictionary<string, string>? Properties { get; set; }
}
