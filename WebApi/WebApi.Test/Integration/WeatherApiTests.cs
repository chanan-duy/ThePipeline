using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Test.Integration;

[TestFixture]
public class WeatherApiTests
{
	private WebApplicationFactory<Program> _factory;
	private Mock<IWeatherForecastService> _mockWeatherService;
	private HttpClient _client;

	[SetUp]
	public void Setup()
	{
		_mockWeatherService = new Mock<IWeatherForecastService>();

		_factory = new WebApplicationFactory<Program>()
			.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IWeatherForecastService));
					if (descriptor != null)
					{
						services.Remove(descriptor);
					}

					services.AddSingleton(_mockWeatherService.Object);
				});
			});

		_client = _factory.CreateClient();
	}

	[TearDown]
	public void TearDown()
	{
		_client.Dispose();
		_factory.Dispose();
	}

	[Test]
	public async Task Get_Weather_Returns_200_OK_With_Json_Content_Type()
	{
		_mockWeatherService.Setup(s => s.GetCurrentWeather())
			.Returns(new List<WeatherForecastDto>());

		var response = await _client.GetAsync("/api/weather/");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
		}
	}

	[Test]
	public async Task Get_Weather_Correctly_Serializes_Service_Data()
	{
		var expectedDate = new DateOnly(2099, 12, 31);
		var mockData = new[]
		{
			new WeatherForecastDto(expectedDate, -5, "Apocalypse"),
		};

		_mockWeatherService.Setup(s => s.GetCurrentWeather()).Returns(mockData);

		var result = await _client.GetFromJsonAsync<WeatherForecastDto[]>("/api/weather/");

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Has.Length.EqualTo(1));
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result![0].Summary, Is.EqualTo("Apocalypse"));
			Assert.That(result[0].Date, Is.EqualTo(expectedDate));
		}
	}

	[Test]
	public async Task Get_Weather_Returns_Empty_Json_Array_When_Service_Returns_Empty()
	{
		_mockWeatherService.Setup(s => s.GetCurrentWeather())
			.Returns([]);

		var result = await _client.GetFromJsonAsync<WeatherForecastDto[]>("/api/weather/");

		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.Empty);
	}

	[Test]
	public async Task Get_Weather_Returns_500_When_Service_Throws_Exception()
	{
		_mockWeatherService.Setup(s => s.GetCurrentWeather())
			.Throws(new InvalidOperationException("Database down"));

		var response = await _client.GetAsync("/api/weather/");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
	}

	[Test]
	public async Task Get_Weather_Route_Is_Not_Case_Sensitive_By_Default()
	{
		_mockWeatherService.Setup(s => s.GetCurrentWeather()).Returns(new List<WeatherForecastDto>());

		var response = await _client.GetAsync("/API/WEATHER/");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
	}
}
