using System.Net.Http.Headers;
using System.Text;
using AzureDevOps.Application.Interfaces;
using AzureDevOps.Infrastructure.Clients;
using AzureDevOps.Infrastructure.Mappings;
using AzureDevOps.Infrastructure.Settings;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOps.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("AzureDevOpsApi", client =>
        {
            client.BaseAddress = new Uri($"https://dev.azure.com/{settings.Organization}/");

            var credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($":{settings.PersonalAccessToken}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        var mappingConfig = new MappingConfig();
        mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        services.AddScoped<IAzureDevOpsClient, AzureDevOpsClient>();

        return services;
    }
}
