using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("TrelloApi", c => c.BaseAddress = new Uri("http://localhost:5001"));
builder.Services.AddHttpClient("MiroApi", c => c.BaseAddress = new Uri("http://localhost:5002"));

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
