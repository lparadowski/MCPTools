namespace Jira.Api.Requests;

public class SearchIssuesRequest
{
    public required string Jql { get; set; }
    public int MaxResults { get; set; } = 50;
}
