using Microsoft.Extensions.DependencyInjection;
using Octopus.Application.Interfaces;
using Octopus.Infrastructure.Clients;
using Octopus.Infrastructure.Settings;

namespace Octopus.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("OctopusApi", client =>
        {
            client.BaseAddress = new Uri(settings.OctopusServerUrl);

            if (!string.IsNullOrWhiteSpace(settings.OctopusApiKey))
            {
                client.DefaultRequestHeaders.Add("X-Octopus-ApiKey", settings.OctopusApiKey);
            }

            client.DefaultRequestHeaders.Add("User-Agent", "McpProjectTools");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = settings.DisableSslValidation
                ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                : null
        });

        services.AddScoped<IOctopusClient, OctopusClient>();

        return services;
    }
}
