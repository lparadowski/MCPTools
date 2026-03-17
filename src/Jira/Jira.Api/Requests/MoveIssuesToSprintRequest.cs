namespace Jira.Api.Requests;

public class MoveIssuesToSprintRequest
{
    public required List<string> IssueKeys { get; set; }
}
