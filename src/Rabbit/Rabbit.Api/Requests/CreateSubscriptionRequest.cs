namespace Rabbit.Api.Requests;

public class CreateSubscriptionRequest
{
    public required string Exchange { get; set; }
    public required string RoutingKey { get; set; }
    public string? QueueName { get; set; }
}
