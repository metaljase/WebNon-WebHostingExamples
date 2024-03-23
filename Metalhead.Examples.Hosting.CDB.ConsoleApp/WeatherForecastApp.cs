using Microsoft.Extensions.Logging;

using Metalhead.Examples.Hosting.Core.Models;
using Metalhead.Examples.Hosting.Core.Services;

namespace Metalhead.Examples.Hosting.CDB.ConsoleApp;

public class WeatherForecastApp(ILogger<WeatherForecastApp> logger, IWeatherService weatherService, WeatherForecastOptions weatherForecastOptions)
{
    private readonly ILogger<WeatherForecastApp> _logger = logger;
    private IWeatherService WeatherService { get; } = weatherService;

    public void GetWeatherForecast() => GetWeatherForecast(weatherForecastOptions.NumberOfDays);

    public void GetWeatherForecast(int numberOfDays)
    {
        _logger.LogInformation("Getting weather forecast.");

        var temperatureScale = weatherForecastOptions.TemperatureScale.ToUpper();
        var windSpeedUnit = weatherForecastOptions.WindSpeedUnit.ToLower();
        var weatherForecast = WeatherService.GetWeatherForecastData(numberOfDays);

        foreach (var forecast in weatherForecast)
        {
            int temperature = weatherForecastOptions.TemperatureScale switch
            {
                "C" => forecast.TemperatureC,
                "F" => forecast.TemperatureF,
                _ => -1
            };

            int windSpeed = weatherForecastOptions.WindSpeedUnit switch
            {
                "MPH" => forecast.WindSpeedMph,
                "KPH" => forecast.WindSpeedKph,
                "KN" => forecast.WindSpeedKnots,
                _ => -1,
            };

            Console.WriteLine($"Date: {forecast.Date} | Temperature: {temperature} °{temperatureScale} | Wind Speed: {windSpeed} {windSpeedUnit}");
        }
    }
}