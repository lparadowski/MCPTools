using Jira.Api.Responses;
using Jira.Domain.Entities;
using Mapster;

namespace Jira.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Project, ProjectResponse>();
        config.NewConfig<Issue, IssueResponse>();
        config.NewConfig<Transition, TransitionResponse>();
        config.NewConfig<Comment, CommentResponse>();
        config.NewConfig<Board, BoardResponse>();
        config.NewConfig<Sprint, SprintResponse>();
        config.NewConfig<Worklog, WorklogResponse>();
        config.NewConfig<TicketProfile, TicketProfileResponse>();
        config.NewConfig<StatusTransition, StatusTransitionResponse>();
        config.NewConfig<WorklogEntry, WorklogEntryResponse>();
    }
}
