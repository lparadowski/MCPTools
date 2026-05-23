using Asp.Versioning;
using Mapster;
using Rabbit.Api.Mappings;
using Rabbit.Application;
using Rabbit.Infrastructure;
using Rabbit.Infrastructure.Settings;
using Serilog;
using Shared.Api.ExceptionHandler;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Rabbit.Api")
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.Seq("http://localhost:5341")
        .Enrich.WithProperty("Application", "Rabbit.Api");
});

// Configure Mapster
var mappingConfig = new MappingConfig();
mappingConfig.Register(TypeAdapterConfig.GlobalSettings);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddApplicationServices();

var infrastructureSettings = builder.Configuration
    .GetSection("InfrastructureSettings")
    .Get<InfrastructureSettings>();

if (infrastructureSettings is null)
{
    throw new InvalidOperationException("InfrastructureSettings configuration is missing");
}

builder.Services.AddInfrastructureServices(infrastructureSettings);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseExceptionHandler();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
