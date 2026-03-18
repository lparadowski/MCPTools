using Jira.Domain.Entities;
using Jira.Infrastructure.Dtos;
using Mapster;

namespace Jira.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<JiraProjectDto, Project>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Key, src => src.Key ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ProjectTypeKey, src => src.ProjectTypeKey)
            .Map(dest => dest.Style, src => src.Style)
            .Map(dest => dest.LeadAccountId, src => src.Lead != null ? src.Lead.AccountId : null)
            .Map(dest => dest.LeadDisplayName, src => src.Lead != null ? src.Lead.DisplayName : null)
            .Map(dest => dest.Url, src => src.Self);

        config.NewConfig<JiraIssueDto, Issue>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Key, src => src.Key ?? string.Empty)
            .Map(dest => dest.Summary, src => src.Fields != null ? src.Fields.Summary ?? string.Empty : string.Empty)
            .Map(dest => dest.Description, src => ExtractPlainText(src.Fields != null ? src.Fields.Description : null))
            .Map(dest => dest.IssueType, src => src.Fields != null && src.Fields.Issuetype != null ? src.Fields.Issuetype.Name ?? string.Empty : string.Empty)
            .Map(dest => dest.Status, src => src.Fields != null && src.Fields.Status != null ? src.Fields.Status.Name ?? string.Empty : string.Empty)
            .Map(dest => dest.Priority, src => src.Fields != null && src.Fields.Priority != null ? src.Fields.Priority.Name : null)
            .Map(dest => dest.AssigneeAccountId, src => src.Fields != null && src.Fields.Assignee != null ? src.Fields.Assignee.AccountId : null)
            .Map(dest => dest.AssigneeDisplayName, src => src.Fields != null && src.Fields.Assignee != null ? src.Fields.Assignee.DisplayName : null)
            .Map(dest => dest.ReporterDisplayName, src => src.Fields != null && src.Fields.Reporter != null ? src.Fields.Reporter.DisplayName : null)
            .Map(dest => dest.ProjectKey, src => src.Fields != null && src.Fields.Project != null ? src.Fields.Project.Key : null)
            .Map(dest => dest.ParentKey, src => src.Fields != null && src.Fields.Parent != null ? src.Fields.Parent.Key : null)
            .Map(dest => dest.Labels, src => src.Fields != null && src.Fields.Labels != null ? src.Fields.Labels : new List<string>())
            .Map(dest => dest.Created, src => src.Fields != null && src.Fields.Created != null ? DateTime.Parse(src.Fields.Created) : (DateTime?)null)
            .Map(dest => dest.Updated, src => src.Fields != null && src.Fields.Updated != null ? DateTime.Parse(src.Fields.Updated) : (DateTime?)null);

        config.NewConfig<JiraTransitionDto, Transition>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.ToStatus, src => src.To != null ? src.To.Name : null);

        config.NewConfig<JiraCommentDto, Comment>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.AuthorDisplayName, src => src.Author != null ? src.Author.DisplayName : null)
            .Map(dest => dest.Body, src => ExtractPlainText(src.Body))
            .Map(dest => dest.Created, src => src.Created != null ? DateTime.Parse(src.Created) : (DateTime?)null)
            .Map(dest => dest.Updated, src => src.Updated != null ? DateTime.Parse(src.Updated) : (DateTime?)null);

        config.NewConfig<JiraBoardDto, Board>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.ProjectKey, src => src.Location != null ? src.Location.ProjectKey : null);

        config.NewConfig<JiraSprintDto, Sprint>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.State, src => src.State)
            .Map(dest => dest.Goal, src => src.Goal)
            .Map(dest => dest.StartDate, src => src.StartDate != null ? DateTime.Parse(src.StartDate) : (DateTime?)null)
            .Map(dest => dest.EndDate, src => src.EndDate != null ? DateTime.Parse(src.EndDate) : (DateTime?)null);
    }

    private static string? ExtractPlainText(JiraDocumentDto? doc)
    {
        if (doc?.Content is null)
        {
            return null;
        }

        var texts = new List<string>();

        foreach (var block in doc.Content)
        {
            if (block.Content is null)
            {
                continue;
            }

            foreach (var inline in block.Content)
            {
                if (inline.Text is not null)
                {
                    texts.Add(inline.Text);
                }
            }
        }

        return texts.Count > 0 ? string.Join(" ", texts) : null;
    }
}
