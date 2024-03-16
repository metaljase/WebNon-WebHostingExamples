using Metalhead.Examples.Hosting.Core.Models;

namespace Metalhead.Examples.Hosting.Core.Services;

public interface IWeatherService
{
    IEnumerable<WeatherForecast> GetWeatherForecastData();
    IEnumerable<WeatherForecast> GetWeatherForecastData(int numberOfDays);
}