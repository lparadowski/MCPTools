using Mapster;
using Miro.Api.Responses;
using Miro.Domain.Entities;

namespace Miro.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Board, BoardResponse>();
        config.NewConfig<StickyNote, StickyNoteResponse>();
    }
}
