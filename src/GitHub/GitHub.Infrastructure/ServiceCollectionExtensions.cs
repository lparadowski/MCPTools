using System.Net.Http.Headers;
using GitHub.Application.Interfaces;
using GitHub.Infrastructure.Clients;
using GitHub.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace GitHub.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("GitHubApi", client =>
        {
            client.BaseAddress = new Uri("https://api.github.com");

            if (!string.IsNullOrWhiteSpace(settings.GitHubToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", settings.GitHubToken);
            }

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Add("User-Agent", "McpProjectTools");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = settings.DisableSslValidation
                ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                : null
        });

        services.AddScoped<IGitHubClient, GitHubClient>();

        return services;
    }
}
