using Mapster;
using Trello.Domain.Entities;
using Trello.Infrastructure.Dtos;

namespace Trello.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TrelloBoardDto, Board>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Description, src => src.Desc)
            .Map(dest => dest.Url, src => src.Url)
            .Ignore(dest => dest.Lists);

        config.NewConfig<TrelloListDto, BoardList>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Ignore(dest => dest.Cards);

        config.NewConfig<TrelloCardDto, Card>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Description, src => src.Desc)
            .Map(dest => dest.ListId, src => src.IdList)
            .Map(dest => dest.Url, src => src.ShortUrl)
            .Map(dest => dest.LastActivity, src => src.DateLastActivity)
            .Map(dest => dest.Labels, src => src.Labels ?? new List<TrelloLabelDto>())
            .Ignore(dest => dest.ListName)
            .Ignore(dest => dest.Comments);

        config.NewConfig<TrelloLabelDto, Label>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Color, src => src.Color);

        config.NewConfig<TrelloActionDto, Comment>()
            .Map(dest => dest.Id, src => src.Id ?? string.Empty)
            .Map(dest => dest.Text, src => src.Data != null ? src.Data.Text : null)
            .Map(dest => dest.Date, src => src.Date);
    }
}
