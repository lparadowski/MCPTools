using Microsoft.Extensions.DependencyInjection;
using Rabbit.Application.Services;

namespace Rabbit.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRabbitService, RabbitService>();

        return services;
    }
}
