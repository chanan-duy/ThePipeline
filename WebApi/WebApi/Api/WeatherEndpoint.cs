using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Api;

public abstract class WeatherEndpointLogger
{
}

public static class WeatherEndpoint
{
	extension(RouteGroupBuilder group)
	{
		public RouteGroupBuilder MapApiWeather()
		{
			group.MapGet("/", GetWeatherForecast);

			return group;
		}
	}

	private static WeatherForecastDto[] GetWeatherForecast(ILogger<WeatherEndpointLogger> logger, IWeatherForecastService weatherService)
	{
		var forecast = weatherService.GetCurrentWeather();

		return forecast.ToArray();
	}
}
