using Microsoft.AspNetCore.Http.HttpResults;

using Metalhead.Examples.Hosting.Core.Models;
using Metalhead.Examples.Hosting.Core.Services;

namespace Metalhead.Examples.Hosting.CDB.Api;

public static class WeatherForecastEndpoints
{
    public static void Register(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/weathersettings", GetWeatherForecastSettings)
            .WithName("GetWeatherSettings")
            .WithOpenApi();

        endpoints.MapGet("/weatherforecastdata", GetWeatherForecastData)
            .WithName("GetWeatherForecastData")
            .WithOpenApi();

        endpoints.MapGet("/weatherforecastdata/{numberOfDays:int}", GetWeatherForecastDataForDays)
            .WithName("GetWeatherForecastDataForDays")
            .WithOpenApi();
    }

    public static Results<Ok<WeatherForecastOptions>, NotFound> GetWeatherForecastSettings(WeatherForecastOptions options)
    {
        return TypedResults.Ok(options);
    }

    public static Results<Ok<IEnumerable<WeatherForecast>>, NotFound> GetWeatherForecastData(IWeatherService weatherService)
    {
        var forecast = weatherService.GetWeatherForecastData();
        return (forecast is null || !forecast.Any()) ? TypedResults.NotFound() : TypedResults.Ok(forecast);
    }

    public static Results<Ok<IEnumerable<WeatherForecast>>, NotFound> GetWeatherForecastDataForDays(IWeatherService weatherService, int numberOfDays)
    {
        var forecast = weatherService.GetWeatherForecastData(numberOfDays);
        return (forecast is null || !forecast.Any()) ? TypedResults.NotFound() : TypedResults.Ok(forecast);
    }
}
