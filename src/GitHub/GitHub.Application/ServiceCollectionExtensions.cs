using GitHub.Application.Interfaces;
using GitHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GitHub.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGitHubService, GitHubService>();
        return services;
    }
}
