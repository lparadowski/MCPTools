using Microsoft.Extensions.DependencyInjection;
using Rabbit.Application.Interfaces;
using Rabbit.Infrastructure.Clients;
using Rabbit.Infrastructure.Settings;

namespace Rabbit.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("RabbitManagementApi", client =>
        {
            client.BaseAddress = new Uri(settings.RabbitMqManagementUri);
        });

        // Singleton: holds the long-lived AMQP connection + the subscription registry.
        services.AddSingleton<IRabbitClient, RabbitClient>();

        return services;
    }
}
