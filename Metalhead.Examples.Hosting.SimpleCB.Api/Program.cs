using Microsoft.Extensions.Options;

using Metalhead.Examples.Hosting.Core.Models;
using Metalhead.Examples.Hosting.Core.Services;
using Metalhead.Examples.Hosting.SimpleCB.Api;

var builder = WebApplication.CreateBuilder(args);
// CreateBuilder has already:
// Set the content root to the path returned by GetCurrentDirectory().
// Loaded host configuration from:
//   Environment variables: AddEnvironmentVariables(prefix: "ASPNETCORE_") & AddEnvironmentVariables(prefix: "DOTNET_").
//   Command line arguments: AddCommandLine(args).
//
// Loaded app configuration from (for overriding app settings in order of lowest to highest priority):
//   appsettings.json: AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).
//   appsettings.{Environment}.json: AddJsonFile($"appsettings.{Environment}.json", optional: true, reloadOnChange: true).
//   User secrets: AddUserSecrets(<assembly>, optional: true, reloadOnChange: true) if the current host environment is Development.
//      i.e. In Visual Studio, right-click on the project and select 'Manage User Secrets'.
//   Environment variables: AddEnvironmentVariables(), so app settings can be overridden using environment variables.
//      e.g. Variable name: weatherForecast:windSpeedUnit / Variable value: MPH
//   Command line arguments: AddCommandLine(args), so app settings can be overridden using CL arguments with nested app settings property names.
//      e.g. dotnet run --weatherForecast:numberOfDays 7 --weatherForecast:temperatureScale F --weatherForecast:windSpeedUnit MPH
//
// Set the web root to the path returned by GetCurrentDirectory().


// Add services to the container.
// Use the Options pattern to bind app settings.  Validation is performed in WeatherForecastOptions using DataAnnotations attributes.
builder.Services.AddOptions<WeatherForecastOptions>()
    .Bind(builder.Configuration.GetSection(WeatherForecastOptions.Settings))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // Validate app settings before running the application.
// Register WeatherForecastOptions by delegating to IOptions object to remove IOptions dependency.
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<WeatherForecastOptions>>().Value);

builder.Services.AddSingleton<IWeatherService, WeatherService>();

// CreateBuilder has already called AddRouting().
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CreateBuilder has already added the Console, Debug, EventLog, and EventSource loggers.
// Add any additional logging configuration to what is specified in appsettings.{Environment}.json.
// e.g. builder.Logging.AddJsonConsole();

using var app = builder.Build();
// CreateBuilder has already called:
//   UseDeveloperExceptionPage() if the current host environment is Development.
//   UseRouting() to add route matching to the middleware pipeline.
//   UseEndpoints() to add endpoint execution to the middleware pipeline.

using var serviceScope = app.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;

ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");    
}

app.UseHttpsRedirection();

// Add WeatherForecast endpoints.
WeatherForecastEndpoints.Register(app);

logger.LogInformation("Application starting...");

try
{
    app.Run();
}
catch (Exception ex)
{
    if (ex is OptionsValidationException)
    {
        // Write validation errors separately to be more obvious.
        logger.LogCritical("Application exited due to invalid app settings:\r\n{ValidationErrors}", ex.Message.Replace("; ", Environment.NewLine));
    }

    logger.LogCritical(ex, "Application exited unexpectedly.");
}