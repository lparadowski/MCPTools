using Microsoft.Extensions.DependencyInjection;
using Octopus.Application.Interfaces;
using Octopus.Application.Services;

namespace Octopus.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOctopusService, OctopusService>();
        return services;
    }
}
