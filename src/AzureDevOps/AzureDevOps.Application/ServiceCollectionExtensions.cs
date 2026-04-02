using AzureDevOps.Application.Interfaces;
using AzureDevOps.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOps.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAzureDevOpsService, AzureDevOpsService>();
        return services;
    }
}
