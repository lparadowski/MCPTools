using Microsoft.Extensions.DependencyInjection;
using Miro.Application.Services;

namespace Miro.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMiroService, MiroService>();

        return services;
    }
}
