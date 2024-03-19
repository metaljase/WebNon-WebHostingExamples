using Microsoft.Extensions.Options;
using Serilog;

using Metalhead.Examples.Hosting.CB.Api;
using Metalhead.Examples.Hosting.Core.Models;
using Metalhead.Examples.Hosting.Core.Services;
using Metalhead.Examples.Hosting.Core;

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


// Allow app settings to be overridden via environment variables, e.g. WEATHER_weatherForecast:temperatureScale
// NOTE: Calling AddEnvironmentVariables again is unnecessary if not prefixing variables, e.g. weatherForecast:temperatureScale
builder.Configuration.AddEnvironmentVariables(prefix: "WEATHER_");

// Because AddEnvironmentVariables has been called above (superseding command line arguments), call AddCommandLine again.
// Allow app settings to be overridden via the command line using single dash or double dash arguments.
// e.g. dotnet run -n 7 -t F --windSpeedUnit MPH (to run in Development environment as HTTPS add '--environment Development --launch-profile https').
var switchMappings = new Dictionary<string, string>
{
    { "--numberOfDays", "weatherForecast:numberOfDays" },
    { "-n", "weatherForecast:numberOfDays" },
    { "--temperatureScale", "weatherForecast:temperatureScale" },
    { "-t", "weatherForecast:temperatureScale" },
    { "--windSpeedUnit", "weatherForecast:windSpeedUnit" },
    { "-w", "weatherForecast:windSpeedUnit" }
};
builder.Configuration.AddCommandLine(args, switchMappings);
// NOTE: If AddCommandLine is called without switch mappings, command line arguments must match nested app settings property names.
// e.g. dotnet run --weatherForecast:numberOfDays 7 --weatherForecast:temperatureScale F --weatherForecast:windSpeedUnit MPH

// Add services to the container.
// Use the Options pattern to bind app settings.  Validation is performed in WeatherForecastOptionsValidation.
builder.Services.AddOptions<WeatherForecastOptions>()
    .Bind(builder.Configuration.GetSection(WeatherForecastOptions.Settings))
    .ValidateOnStart(); // Validate app settings before running the application.
builder.Services.AddSingleton<IValidateOptions<WeatherForecastOptions>, WeatherForecastOptionsValidation>();
// Register WeatherForecastOptions by delegating to IOptions object to remove IOptions dependency.
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<WeatherForecastOptions>>().Value);

builder.Services.AddSingleton<IWeatherService, WeatherService>();

// CreateBuilder has already called AddRouting().
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CreateBuilder has already added the Console, Debug, EventLog, and EventSource loggers.
// Add any additional logging configuration to what is specified in appsettings.{Environment}.json.
// e.g. builder.Logging.AddJsonConsole();
// Use Serilog for logging instead.  Serilog is configured in appsettings.{Environment}.json to write to the console and a log file.
builder.Logging.ClearProviders().AddSerilog();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

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
    Log.Logger.Fatal(ex, "Application exited unexpectedly.  See log file for details.");

    if (ex is OptionsValidationException)
    {
        // Write validation errors separately to be more obvious.
        Log.Logger.Fatal("Application exited due to invalid app settings:\r\n{ValidationErrors}", ex.Message.Replace("; ", Environment.NewLine));
    }
}
finally
{
    Log.CloseAndFlush();
}
