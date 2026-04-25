using Mapster;
using Miro.Domain.Entities;
using Miro.Infrastructure.Dtos;

namespace Miro.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MiroBoardDto, Board>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.ModifiedAt, src => src.ModifiedAt);

        config.NewConfig<MiroItemDto, StickyNote>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Data != null ? src.Data.Content : null)
            .Map(dest => dest.Shape, src => src.Data != null ? src.Data.Shape : null)
            .Map(dest => dest.FillColor, src => src.Style != null ? src.Style.FillColor : null)
            .Map(dest => dest.PositionX, src => src.Position != null ? src.Position.X : null)
            .Map(dest => dest.PositionY, src => src.Position != null ? src.Position.Y : null)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.ModifiedAt, src => src.ModifiedAt);
    }
}
