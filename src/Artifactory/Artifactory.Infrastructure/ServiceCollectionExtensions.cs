using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Artifactory.Application.Interfaces;
using Artifactory.Infrastructure.Clients;
using Artifactory.Infrastructure.Settings;

namespace Artifactory.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("ArtifactoryApi", client =>
        {
            client.BaseAddress = new Uri(settings.ArtifactoryBaseUrl);

            if (!string.IsNullOrWhiteSpace(settings.ArtifactoryPat))
            {
                // JFrog personal access tokens are bearer tokens.
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", settings.ArtifactoryPat);
            }

            client.DefaultRequestHeaders.Add("User-Agent", "McpProjectTools");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = settings.DisableSslValidation
                ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                : null
        });

        services.AddScoped<IArtifactoryClient, ArtifactoryClient>();

        return services;
    }
}
