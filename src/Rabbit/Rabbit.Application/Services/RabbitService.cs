using FluentResults;
using Rabbit.Application.Interfaces;
using Rabbit.Domain.Entities;
using Shared.Application.ResultErrors;

namespace Rabbit.Application.Services;

public interface IRabbitService
{
    // Subscriptions
    Task<Result<Subscription>> SubscribeAsync(string exchange, string routingKey, string? queueName, CancellationToken cancellationToken = default);
    Task<Result<List<Message>>> DrainAsync(string subscriptionId, int maxMessages, int waitMs, CancellationToken cancellationToken = default);
    Task<Result> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default);
    Task<Result<List<Subscription>>> ListSubscriptionsAsync(CancellationToken cancellationToken = default);

    // Publishing
    Task<Result> PublishAsync(string exchange, string routingKey, string body, string? contentType, Dictionary<string, string>? headers, CancellationToken cancellationToken = default);

    // Broker introspection
    Task<Result<List<Exchange>>> ListExchangesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Queue>>> ListQueuesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Message>>> PeekAsync(string queueName, int count, CancellationToken cancellationToken = default);
}

public class RabbitService(IRabbitClient rabbitClient) : IRabbitService
{
    public async Task<Result<Subscription>> SubscribeAsync(string exchange, string routingKey, string? queueName, CancellationToken cancellationToken = default)
    {
        var subscription = await rabbitClient.SubscribeAsync(exchange, routingKey, queueName, cancellationToken);
        return Result.Ok(subscription);
    }

    public async Task<Result<List<Message>>> DrainAsync(string subscriptionId, int maxMessages, int waitMs, CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await rabbitClient.DrainAsync(subscriptionId, maxMessages, waitMs, cancellationToken);
            return Result.Ok(messages);
        }
        catch (KeyNotFoundException)
        {
            return Result.Fail<List<Message>>(new EntityDoesNotExistError());
        }
    }

    public async Task<Result> UnsubscribeAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        var removed = await rabbitClient.UnsubscribeAsync(subscriptionId, cancellationToken);
        return removed ? Result.Ok() : Result.Fail(new EntityDoesNotExistError());
    }

    public Task<Result<List<Subscription>>> ListSubscriptionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok(rabbitClient.ListSubscriptions().ToList()));
    }

    public async Task<Result> PublishAsync(string exchange, string routingKey, string body, string? contentType, Dictionary<string, string>? headers, CancellationToken cancellationToken = default)
    {
        await rabbitClient.PublishAsync(exchange, routingKey, body, contentType, headers, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result<List<Exchange>>> ListExchangesAsync(CancellationToken cancellationToken = default)
    {
        var exchanges = await rabbitClient.ListExchangesAsync(cancellationToken);
        return Result.Ok(exchanges);
    }

    public async Task<Result<List<Queue>>> ListQueuesAsync(CancellationToken cancellationToken = default)
    {
        var queues = await rabbitClient.ListQueuesAsync(cancellationToken);
        return Result.Ok(queues);
    }

    public async Task<Result<List<Message>>> PeekAsync(string queueName, int count, CancellationToken cancellationToken = default)
    {
        var messages = await rabbitClient.PeekAsync(queueName, count, cancellationToken);
        return Result.Ok(messages);
    }
}
