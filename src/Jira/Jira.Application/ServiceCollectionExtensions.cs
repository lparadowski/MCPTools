using Jira.Application.Interfaces;
using Jira.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Jira.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJiraService, JiraService>();

        return services;
    }
}
