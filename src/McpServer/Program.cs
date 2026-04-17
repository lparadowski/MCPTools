using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();

var timeout = TimeSpan.FromSeconds(30);
builder.Services.AddHttpClient("TrelloApi", c => { c.BaseAddress = new Uri("http://localhost:5001"); c.Timeout = timeout; });
builder.Services.AddHttpClient("MiroApi", c => { c.BaseAddress = new Uri("http://localhost:5002"); c.Timeout = timeout; });
builder.Services.AddHttpClient("ConfluenceApi", c => { c.BaseAddress = new Uri("http://localhost:5003"); c.Timeout = timeout; });
builder.Services.AddHttpClient("JiraApi", c => { c.BaseAddress = new Uri("http://localhost:5004"); c.Timeout = timeout; });
builder.Services.AddHttpClient("AzureDevOpsApi", c => { c.BaseAddress = new Uri("http://localhost:5005"); c.Timeout = timeout; });
builder.Services.AddHttpClient("PolarionApi", c => { c.BaseAddress = new Uri("http://localhost:5006"); c.Timeout = timeout; });
builder.Services.AddHttpClient("GitHubApi", c => { c.BaseAddress = new Uri("http://localhost:5007"); c.Timeout = timeout; });

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
