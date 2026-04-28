namespace GitHub.Api.Responses;

public class PullRequestResponse
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
    public int? Additions { get; set; }
    public int? Deletions { get; set; }
    public int? ChangedFiles { get; set; }
    public int? ReviewComments { get; set; }
}
