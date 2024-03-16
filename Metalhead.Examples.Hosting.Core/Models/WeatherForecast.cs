namespace Metalhead.Examples.Hosting.Core.Models;

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public int WindSpeedKnots { get; set; }

    public int WindSpeedMph => (int)Math.Round(WindSpeedKnots * 1.15078);
    public int WindSpeedKph => (int)Math.Round(WindSpeedKnots * 1.852);
}
