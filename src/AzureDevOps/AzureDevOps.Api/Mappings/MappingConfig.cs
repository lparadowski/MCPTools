using AzureDevOps.Api.Responses;
using AzureDevOps.Domain.Entities;
using Mapster;

namespace AzureDevOps.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Project, ProjectResponse>();
        config.NewConfig<WorkItem, WorkItemResponse>();
        config.NewConfig<Team, TeamResponse>();
        config.NewConfig<Board, BoardResponse>();
        config.NewConfig<Sprint, SprintResponse>();
        config.NewConfig<Comment, CommentResponse>();
    }
}
