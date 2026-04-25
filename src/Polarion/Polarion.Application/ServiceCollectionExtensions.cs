using Polarion.Application.Interfaces;
using Polarion.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Polarion.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPolarionService, PolarionService>();

        return services;
    }
}
