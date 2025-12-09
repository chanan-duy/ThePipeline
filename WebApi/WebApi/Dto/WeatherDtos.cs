using JetBrains.Annotations;

namespace WebApi.Dto;

public record WeatherForecastDto([UsedImplicitly] DateOnly Date, int TemperatureC, string? Summary)
{
	[UsedImplicitly] public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
