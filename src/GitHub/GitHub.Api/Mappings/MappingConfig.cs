using GitHub.Api.Responses;
using GitHub.Domain.Entities;
using Mapster;

namespace GitHub.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Repository, RepositoryResponse>();
        config.NewConfig<ActivityEvent, ActivityEventResponse>();
    }
}
