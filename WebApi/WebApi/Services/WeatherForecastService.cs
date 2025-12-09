using WebApi.Dto;

namespace WebApi.Services;

public class WeatherForecastService(ILogger<WeatherForecastService> logger) : IWeatherForecastService
{
	public IEnumerable<WeatherForecastDto> GetCurrentWeather()
	{
		var summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "Unknown",
		};

		var forecast = Enumerable.Range(1, 5).Select(index =>
			new WeatherForecastDto
			(
				DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				Random.Shared.Next(-20, 55),
				summaries[Random.Shared.Next(summaries.Length)]
			));

		return forecast;
	}
}
