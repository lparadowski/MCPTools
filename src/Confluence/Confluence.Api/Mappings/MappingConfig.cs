using Confluence.Api.Responses;
using Confluence.Domain.Entities;
using Mapster;

namespace Confluence.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Space, SpaceResponse>();
        config.NewConfig<Page, PageResponse>();
        config.NewConfig<SearchResult, SearchResultResponse>();
    }
}
