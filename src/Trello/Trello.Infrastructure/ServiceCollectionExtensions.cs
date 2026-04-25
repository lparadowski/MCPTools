using Mapster;
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

        services.AddHttpClient("TrelloApi", client =>
        {
            client.BaseAddress = new Uri("https://api.trello.com");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new AuthQueryHandler(settings);

            // Corporate proxies may intercept HTTPS with a custom root CA that the
            // container doesn't trust. Set DISABLE_SSL_VALIDATION=true to bypass.
            // Only use this in development/corporate environments — never in production.
            if (settings.DisableSslValidation)
            {
                handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            }

            return handler;
        });

        var mappingConfig = new MappingConfig();
        mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        services.AddScoped<ITrelloClient, TrelloClient>();

        return services;
    }

    private class AuthQueryHandler(InfrastructureSettings settings) : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri!);
            var separator = string.IsNullOrEmpty(uriBuilder.Query) ? "?" : "&";
            uriBuilder.Query = uriBuilder.Query.TrimStart('?')
                + $"{separator}key={Uri.EscapeDataString(settings.TrelloApiKey)}"
                + $"&token={Uri.EscapeDataString(settings.TrelloApiToken)}";
            request.RequestUri = uriBuilder.Uri;

            return base.SendAsync(request, cancellationToken);
        }
    }
}
