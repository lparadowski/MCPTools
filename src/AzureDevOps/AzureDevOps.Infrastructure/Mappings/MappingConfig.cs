using AzureDevOps.Domain.Entities;
using AzureDevOps.Infrastructure.Dtos;
using Mapster;

namespace AzureDevOps.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProjectDto, Project>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.State, src => src.State ?? string.Empty);

        config.NewConfig<WorkItemDto, WorkItem>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Fields != null ? src.Fields.Title ?? string.Empty : string.Empty)
            .Map(dest => dest.Description, src => src.Fields != null ? src.Fields.Description : null)
            .Map(dest => dest.WorkItemType, src => src.Fields != null ? src.Fields.WorkItemType ?? string.Empty : string.Empty)
            .Map(dest => dest.State, src => src.Fields != null ? src.Fields.State ?? string.Empty : string.Empty)
            .Map(dest => dest.AssignedTo, src => src.Fields != null && src.Fields.AssignedTo != null ? src.Fields.AssignedTo.DisplayName : null)
            .Map(dest => dest.AreaPath, src => src.Fields != null ? src.Fields.AreaPath : null)
            .Map(dest => dest.IterationPath, src => src.Fields != null ? src.Fields.IterationPath : null)
            .Map(dest => dest.Priority, src => src.Fields != null && src.Fields.Priority != null ? src.Fields.Priority.ToString() : null)
            .Map(dest => dest.Tags, src => src.Fields != null ? src.Fields.Tags : null)
            .Map(dest => dest.ParentId, src => src.Fields != null ? src.Fields.Parent : null)
            .Map(dest => dest.ProjectName, src => src.Fields != null ? src.Fields.TeamProject : null)
            .Map(dest => dest.CreatedDate, src => src.Fields != null ? src.Fields.CreatedDate : null)
            .Map(dest => dest.ChangedDate, src => src.Fields != null ? src.Fields.ChangedDate : null)
            .Map(dest => dest.Url, src => src.Url);

        config.NewConfig<TeamDto, Team>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty);

        config.NewConfig<BoardDto, Board>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty);

        config.NewConfig<SprintDto, Sprint>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Path, src => src.Path)
            .Map(dest => dest.StartDate, src => src.Attributes != null ? src.Attributes.StartDate : null)
            .Map(dest => dest.FinishDate, src => src.Attributes != null ? src.Attributes.FinishDate : null)
            .Map(dest => dest.TimeFrame, src => src.Attributes != null ? src.Attributes.TimeFrame : null);

        config.NewConfig<CommentDto, Comment>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Text, src => src.Text ?? string.Empty)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy != null ? src.CreatedBy.DisplayName : null)
            .Map(dest => dest.CreatedDate, src => src.CreatedDate);
    }
}
