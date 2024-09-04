using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

using Metalhead.Examples.Hosting.Core.Models;

namespace Metalhead.Examples.Hosting.Core;

public class WeatherForecastOptionsValidation() : IValidateOptions<WeatherForecastOptions>
{
    public ValidateOptionsResult Validate(string? name, WeatherForecastOptions options)
    {
        List<ValidationResult> validationResults = [];

        if (options.NumberOfDays < 1 || options.NumberOfDays > 7)
        {
            validationResults.Add(new ValidationResult($"{nameof(WeatherForecastOptions.NumberOfDays)} must be between 1 and 7."));
        }

        if (string.IsNullOrWhiteSpace(options.TemperatureScale))
        {
            validationResults.Add(new ValidationResult($"{nameof(WeatherForecastOptions.TemperatureScale)} is not specified in app settings."));
        }
        else if (options.TemperatureScale != "C" && options.TemperatureScale != "F")
        {
            validationResults.Add(new ValidationResult($"{nameof(WeatherForecastOptions.TemperatureScale)} must be either 'C' or 'F'."));
        }

        if (string.IsNullOrWhiteSpace(options.WindSpeedUnit))
        {
            validationResults.Add(new ValidationResult($"{nameof(WeatherForecastOptions.WindSpeedUnit)} is not specified in app settings."));
        }
        else if (options.WindSpeedUnit != "MPH" && options.WindSpeedUnit != "KPH" && options.WindSpeedUnit != "KN")
        {
            validationResults.Add(new ValidationResult($"{nameof(WeatherForecastOptions.WindSpeedUnit)} must be either 'MPH', 'KPH', or 'KN'."));
        }

        if (validationResults.Count > 0)
        {
            var failures = validationResults.Where(v => v.ErrorMessage is not null).Select(v => v.ErrorMessage!);
            return ValidateOptionsResult.Fail(failures);
        }

        return ValidateOptionsResult.Success;
    }
}