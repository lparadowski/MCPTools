using Mapster;
using Trello.Api.Responses;
using Trello.Domain.Entities;

namespace Trello.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Board, BoardResponse>();
        config.NewConfig<BoardList, BoardListResponse>();
        config.NewConfig<Card, CardResponse>();
        config.NewConfig<Label, LabelResponse>();
        config.NewConfig<Comment, CommentResponse>();
    }
}
