using Mapster;
using Octopus.Api.Responses;
using Octopus.Domain.Entities;

namespace Octopus.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DeployedRelease, DeployedReleaseResponse>();
        config.NewConfig<DeploymentEnvironment, EnvironmentResponse>();
        config.NewConfig<Project, ProjectResponse>();
    }
}
