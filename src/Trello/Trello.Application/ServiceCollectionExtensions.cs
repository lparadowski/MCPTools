using Microsoft.Extensions.DependencyInjection;
using Trello.Application.Services;

namespace Trello.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITrelloService, TrelloService>();

        return services;
    }
}
