using Microsoft.Extensions.DependencyInjection;
using Artifactory.Application.Interfaces;
using Artifactory.Application.Services;

namespace Artifactory.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IArtifactoryService, ArtifactoryService>();
        return services;
    }
}
