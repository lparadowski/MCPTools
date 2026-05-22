using Mapster;
using Rabbit.Api.Responses;
using Rabbit.Domain.Entities;

namespace Rabbit.Api.Mappings;

public class MappingConfig
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Subscription, SubscriptionResponse>();
        config.NewConfig<Message, MessageResponse>();
        config.NewConfig<Exchange, ExchangeResponse>();
        config.NewConfig<Queue, QueueResponse>();
    }
}
