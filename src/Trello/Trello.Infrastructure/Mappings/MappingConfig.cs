using Mapster;
using Manatee.Trello;
using DomainBoard = Trello.Domain.Entities.Board;
using DomainBoardList = Trello.Domain.Entities.BoardList;
using DomainCard = Trello.Domain.Entities.Card;
using DomainLabel = Trello.Domain.Entities.Label;
using DomainComment = Trello.Domain.Entities.Comment;

namespace Trello.Infrastructure.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<IBoard, DomainBoard>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Url, src => src.Url)
            .Ignore(dest => dest.Lists);

        config.NewConfig<IList, DomainBoardList>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Ignore(dest => dest.Cards);

        config.NewConfig<ICard, DomainCard>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name ?? string.Empty)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.ListName, src => src.List != null ? src.List.Name : null)
            .Map(dest => dest.ListId, src => src.List != null ? src.List.Id : null)
            .Map(dest => dest.Url, src => src.ShortUrl)
            .Map(dest => dest.LastActivity, src => src.LastActivity)
            .Map(dest => dest.Labels, src => src.Labels)
            .Ignore(dest => dest.Comments);

        config.NewConfig<ILabel, DomainLabel>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Color, src => src.Color != null ? src.Color.ToString() : null);

        config.NewConfig<IAction, DomainComment>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Text, src => src.Data != null ? src.Data.Text : null)
            .Map(dest => dest.Date, src => src.Date);
    }
}
