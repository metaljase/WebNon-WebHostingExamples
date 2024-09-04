using System.ComponentModel.DataAnnotations;

namespace Metalhead.Examples.Hosting.Core.Models;

public class WeatherForecastOptions
{
    public const string Settings = "WeatherForecast";

    // NOTE: Data Annotation attributes below are required when using '.ValidateDataAnnotations()' for validation...
    //  i.e. SimpleCAB.ConsoleApp, SimpleCB.Api, SimpleCDB.Api, and SimpleCDB.ConsoleApp.

    [Required(ErrorMessage = $"{nameof(NumberOfDays)} is required.")]
    [Range(1, 7, ErrorMessage = $"{nameof(NumberOfDays)} must be between 1 and 7.")]
    public required int NumberOfDays { get; set; }

    [Required(ErrorMessage = $"{nameof(TemperatureScale)} is required.")]
    [AllowedValues("C", "F", ErrorMessage = $"{nameof(TemperatureScale)} must be either 'C' or 'F'.")]
    public required string TemperatureScale { get; set; }

    [Required(ErrorMessage = $"{nameof(WindSpeedUnit)} is required.")]
    [AllowedValues("MPH", "KPH", "KN", ErrorMessage = $"{nameof(WindSpeedUnit)} must be either 'MPH', 'KPH', or 'KN'.")]
    public string WindSpeedUnit { get; set; } = string.Empty;
}