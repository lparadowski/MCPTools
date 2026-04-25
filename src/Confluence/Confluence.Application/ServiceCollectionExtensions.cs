using Confluence.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Confluence.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IConfluenceService, ConfluenceService>();

        return services;
    }
}
