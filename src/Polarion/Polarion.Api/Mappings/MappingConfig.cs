using Polarion.Api.Responses;
using Polarion.Domain.Entities;
using Mapster;

namespace Polarion.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Project, ProjectResponse>();
        config.NewConfig<Requirement, RequirementResponse>();
        config.NewConfig<LinkedWorkItem, LinkedWorkItemResponse>();
    }
}
