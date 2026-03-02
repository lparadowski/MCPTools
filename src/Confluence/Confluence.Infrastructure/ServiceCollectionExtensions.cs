using System.Net.Http.Headers;
using System.Text;
using Confluence.Application.Interfaces;
using Confluence.Infrastructure.Clients;
using Confluence.Infrastructure.Mappings;
using Confluence.Infrastructure.Settings;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Confluence.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("ConfluenceApi", client =>
        {
            client.BaseAddress = new Uri(settings.ConfluenceBaseUrl);
            var credentials = Convert.ToBase64String(
                Encoding.ASCII.GetBytes($"{settings.ConfluenceEmail}:{settings.ConfluenceApiToken}"));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        var mappingConfig = new MappingConfig();
        mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        services.AddScoped<IConfluenceClient, ConfluenceClient>();

        return services;
    }
}
