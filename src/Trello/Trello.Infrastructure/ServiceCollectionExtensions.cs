using Mapster;
using Manatee.Trello;
using Microsoft.Extensions.DependencyInjection;
using Trello.Application.Interfaces;
using Trello.Infrastructure.Clients;
using Trello.Infrastructure.Mappings;
using Trello.Infrastructure.Settings;

namespace Trello.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        TrelloAuthorization.Default.AppKey = settings.TrelloApiKey;
        TrelloAuthorization.Default.UserToken = settings.TrelloApiToken;

        var mappingConfig = new MappingConfig();
        mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        services.AddScoped<ITrelloClient, TrelloClient>();

        return services;
    }
}
