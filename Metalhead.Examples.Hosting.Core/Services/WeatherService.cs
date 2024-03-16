using Microsoft.Extensions.Logging;

using Metalhead.Examples.Hosting.Core.Models;

namespace Metalhead.Examples.Hosting.Core.Services;

public class WeatherService(ILogger<WeatherService> logger, WeatherForecastOptions options) : IWeatherService
{
    private readonly ILogger<WeatherService> _logger = logger;
    private readonly WeatherForecastOptions _options = options;

    public IEnumerable<WeatherForecast> GetWeatherForecastData() => GetWeatherForecastData(_options.NumberOfDays);

    public IEnumerable<WeatherForecast> GetWeatherForecastData(int numberOfDays)
    {
        _logger.LogInformation("Weather service called.");
        
        var startDate = DateOnly.FromDateTime(DateTime.Now);

        return Enumerable.Range(1, numberOfDays).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-10, 55),
            WindSpeedKnots = Random.Shared.Next(0, 60)
        });
    }
}
