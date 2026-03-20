using Asp.Versioning;
using Shared.Api.ExceptionHandler;
using Polarion.Api.Mappings;
using Polarion.Application;
using Polarion.Infrastructure;
using Polarion.Infrastructure.Settings;
using Mapster;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "Polarion.Api")
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Seq("http://localhost:5341")
        .Enrich.WithProperty("Application", "Polarion.Api");
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

// Add health checks
builder.Services.AddHealthChecks();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add application services
builder.Services.AddApplicationServices();

// Add infrastructure services
var infrastructureSettings = builder.Configuration
    .GetSection("InfrastructureSettings")
    .Get<InfrastructureSettings>();

if (infrastructureSettings == null)
{
    throw new InvalidOperationException("InfrastructureSettings configuration is missing");
}

builder.Services.AddInfrastructureServices(infrastructureSettings);

// Add exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add CORS
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

// Configure the HTTP request pipeline
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
