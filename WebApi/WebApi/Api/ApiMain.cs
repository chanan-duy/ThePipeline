namespace WebApi.Api;

public static class ApiMain
{
	extension(RouteGroupBuilder group)
	{
		public RouteGroupBuilder MapApi()
		{
			group.MapGroup("/weather").MapApiWeather().WithTags("Weather");

			return group;
		}
	}
}
