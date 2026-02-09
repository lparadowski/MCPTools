using System.Net.Http.Headers;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Miro.Application.Interfaces;
using Miro.Infrastructure.Clients;
using Miro.Infrastructure.Mappings;
using Miro.Infrastructure.Settings;

namespace Miro.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("MiroApi", client =>
        {
            client.BaseAddress = new Uri("https://api.miro.com");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", settings.MiroAccessToken);
        });

        var mappingConfig = new MappingConfig();
        mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        services.AddScoped<IMiroClient, MiroClient>();

        return services;
    }
}
