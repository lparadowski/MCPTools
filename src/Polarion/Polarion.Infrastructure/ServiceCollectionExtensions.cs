using System.Net.Http.Headers;
using System.Text;
using Polarion.Application.Interfaces;
using Polarion.Infrastructure.Clients;
using Polarion.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Polarion.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        InfrastructureSettings settings)
    {
        services.AddSingleton(settings);

        services.AddHttpClient("PolarionApi", client =>
        {
            client.BaseAddress = new Uri(settings.PolarionBaseUrl);

            if (!string.IsNullOrWhiteSpace(settings.PolarionToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", settings.PolarionToken);
            }
            else if (!string.IsNullOrWhiteSpace(settings.PolarionUsername) &&
                     !string.IsNullOrWhiteSpace(settings.PolarionPassword))
            {
                var credentials = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{settings.PolarionUsername}:{settings.PolarionPassword}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);
            }

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddScoped<IPolarionClient, PolarionClient>();

        return services;
    }
}
