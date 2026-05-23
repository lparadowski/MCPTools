namespace Rabbit.Api.Responses;

public class SubscriptionResponse
{
    public required string Id { get; set; }
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public required string QueueName { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
}
