namespace Jira.Api.Responses;

public class StatusTransitionResponse
{
    public required string FromStatus { get; set; }
    public required string ToStatus { get; set; }
    public DateTime TransitionedAt { get; set; }
}
