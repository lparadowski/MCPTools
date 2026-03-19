using Jira.Api.Responses;
using Jira.Domain.Entities;
using Mapster;

namespace Jira.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Project, ProjectResponse>();
        config.NewConfig<IssueField, IssueFieldResponse>();
        config.NewConfig<Issue, IssueResponse>();
        config.NewConfig<Transition, TransitionResponse>();
        config.NewConfig<Comment, CommentResponse>();
        config.NewConfig<Board, BoardResponse>();
        config.NewConfig<Sprint, SprintResponse>();
    }
}
