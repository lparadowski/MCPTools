namespace Jira.Domain.Entities;

public class StatusTransition
{
    public required string FromStatus { get; set; }
    public required string ToStatus { get; set; }
    public DateTime TransitionedAt { get; set; }
}
