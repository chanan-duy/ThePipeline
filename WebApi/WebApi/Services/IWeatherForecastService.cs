using WebApi.Dto;

namespace WebApi.Services;

public interface IWeatherForecastService
{
	IEnumerable<WeatherForecastDto> GetCurrentWeather();
}
