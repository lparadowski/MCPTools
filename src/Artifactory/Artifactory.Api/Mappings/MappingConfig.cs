using Mapster;
using Artifactory.Api.Responses;
using Artifactory.Domain.Entities;

namespace Artifactory.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ArtifactItem, ArtifactItemResponse>();
        config.NewConfig<BuildSummary, BuildSummaryResponse>();
        config.NewConfig<BuildDetail, BuildDetailResponse>();
    }
}
