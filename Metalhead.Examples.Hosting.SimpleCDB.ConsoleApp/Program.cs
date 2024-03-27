using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Metalhead.Examples.Hosting.Core.Models;
using Metalhead.Examples.Hosting.Core.Services;
using Metalhead.Examples.Hosting.SimpleCDB.ConsoleApp;

var builder = Host
    .CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configurationBuilder =>
    {
        // CreateDefaultBuilder has already:
        // Set the content root to the path returned by GetCurrentDirectory().
        // Loaded host configuration from:
        //   Environment variables: AddEnvironmentVariables(prefix: "DOTNET_").
        //   Command line arguments: AddCommandLine(args).
    })
    .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
    {
        // CreateDefaultBuilder has already:
        // Loaded app configuration from (for overriding app settings in order of lowest to highest priority):
        //   appsettings.json: AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).
        //   appsettings.{Environment}.json: AddJsonFile($"appsettings.{Environment}.json", optional: true, reloadOnChange: true).
        //   User secrets: AddUserSecrets(<assembly>, optional: true, reloadOnChange: true) if the current host environment is Development.
        //      i.e. In Visual Studio, right-click on the project and select 'Manage User Secrets'.
        //   Environment variables: AddEnvironmentVariables(), so app settings can be overridden using environment variables.
        //      e.g. Variable name: weatherForecast:windSpeedUnit / Variable value: MPH
        //   Command line arguments: AddCommandLine(args), so app settings can be overridden using CL arguments with nested app settings property names.
        //      e.g. <appname>.exe --weatherForecast:numberOfDays 7 --weatherForecast:temperatureScale F --weatherForecast:windSpeedUnit MPH
    })
    .ConfigureServices((hostBuilderContext, services) =>
    {
        // Use the Options pattern to bind app settings.  Validation is performed in WeatherForecastOptions using DataAnnotations attributes.
        services.AddOptions<WeatherForecastOptions>()
            .Bind(hostBuilderContext.Configuration.GetSection(WeatherForecastOptions.Settings))
            .ValidateDataAnnotations();
        // Register WeatherForecastOptions by delegating to IOptions object to remove IOptions dependency.
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<WeatherForecastOptions>>().Value);

        services.AddSingleton<WeatherForecastApp>();
        services.AddSingleton<IWeatherService, WeatherService>();
    })
    .ConfigureLogging((hostBuilderContext, logging) =>
    {
        // CreateDefaultBuilder has already added the Console, Debug, EventLog, and EventSource loggers.
        // Add any additional logging configuration to what is specified in appsettings.{Environment}.json.
        // e.g. logging.AddJsonConsole();
    });

using var host = builder.Build();

using var serviceScope = host.Services.CreateScope();
var serviceProvider = serviceScope.ServiceProvider;

ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting...");

try
{
    serviceProvider.GetRequiredService<WeatherForecastApp>().GetWeatherForecast();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application exited unexpectedly.");

    if (ex is OptionsValidationException)
    {
        // Write validation errors separately to be more obvious.
        logger.LogCritical("Application exited due to invalid app settings:\r\n{ValidationErrors}", ex.Message.Replace("; ", Environment.NewLine));
    }
}

Console.WriteLine();
Console.WriteLine("Press any key to exit application.");
Console.ReadKey();
