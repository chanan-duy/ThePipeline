using Microsoft.Extensions.Logging;
using Moq;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Test.Unit;

[TestFixture]
public class WeatherDtoTests
{
	[TestCase(0, 32)]
	[TestCase(100, 211)]
	[TestCase(-10, 15)]
	[TestCase(37, 98)]
	public void TemperatureF_Calculates_Correctly_For_Inputs(int tempC, int expectedF)
	{
		var dto = new WeatherForecastDto(DateOnly.FromDateTime(DateTime.Now), tempC, "Test");

		var actualF = dto.TemperatureF;

		Assert.That(actualF, Is.EqualTo(expectedF));
	}

	[Test]
	public void Constructor_Accepts_Null_Summary()
	{
		var dto = new WeatherForecastDto(DateOnly.FromDateTime(DateTime.Now), 20, null);

		Assert.That(dto.Summary, Is.Null);
		Assert.DoesNotThrow(() =>
		{
			var x = dto.TemperatureF;
		});
	}
}

[TestFixture]
public class WeatherServiceTests
{
	private Mock<ILogger<WeatherForecastService>> _mockLogger;
	private WeatherForecastService _service;

	[SetUp]
	public void Init()
	{
		_mockLogger = new Mock<ILogger<WeatherForecastService>>();
		_service = new WeatherForecastService(_mockLogger.Object);
	}

	[Test]
	public void GetCurrentWeather_Always_Returns_Exactly_Five_Items()
	{
		var result = _service.GetCurrentWeather().ToList();
		Assert.That(result, Has.Count.EqualTo(5));
	}

	[Test]
	public void GetCurrentWeather_Temperatures_Are_Within_Reasonable_Bounds()
	{
		var result = _service.GetCurrentWeather().ToList();

		Assert.That(result, Is.Not.Empty);
		foreach (var item in result)
		{
			Assert.That(item.TemperatureC, Is.GreaterThanOrEqualTo(-20));
			Assert.That(item.TemperatureC, Is.LessThan(55));
		}
	}

	[Test]
	public void GetCurrentWeather_Returns_Sequential_Dates_Starting_Tomorrow()
	{
		var result = _service.GetCurrentWeather().ToList();

		var tomorrow = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

		Assert.That(result[0].Date, Is.EqualTo(tomorrow), "First forecast should be for tomorrow");

		for (var i = 1; i < result.Count; i++)
		{
			var previousDate = result[i - 1].Date;
			var currentDate = result[i].Date;

			var diff = currentDate.DayNumber - previousDate.DayNumber;
			Assert.That(diff, Is.EqualTo(1), $"Dates at index {i} are not sequential");
		}
	}
}
