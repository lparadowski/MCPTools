using Confluence.Domain.Entities;
using Confluence.Infrastructure.Dtos;
using Mapster;

namespace Confluence.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ConfluenceSpaceDto, Space>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Key, src => src.Key)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description != null && src.Description.Plain != null
                ? src.Description.Plain.Value
                : null)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Type, src => src.Type)
            .Map(dest => dest.HomepageId, src => src.HomepageId);

        config.NewConfig<ConfluencePageDto, Page>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.SpaceId, src => src.SpaceId)
            .Map(dest => dest.ParentId, src => src.ParentId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Body, src => src.Body != null && src.Body.Storage != null
                ? src.Body.Storage.Value
                : null)
            .Map(dest => dest.Version, src => src.Version != null ? src.Version.Number : 0);
    }
}
