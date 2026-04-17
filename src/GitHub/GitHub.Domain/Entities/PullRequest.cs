namespace GitHub.Domain.Entities;

public class PullRequest
{
    public required int Number { get; set; }
    public required string Title { get; set; }
    public required string State { get; set; }
    public string? Author { get; set; }
    public string? Body { get; set; }
    public string? HeadBranch { get; set; }
    public string? BaseBranch { get; set; }
    public bool Draft { get; set; }
    public string? Url { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? MergedAt { get; set; }
}
