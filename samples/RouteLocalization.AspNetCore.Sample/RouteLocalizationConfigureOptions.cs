namespace RouteLocalization.AspNetCore.Sample
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Options;

	public class RouteLocalizationConfigureOptions : IConfigureOptions<MvcOptions>
	{
		public RouteLocalizationConfigureOptions(RouteLocalizationConvention convention)
		{
			Convention = convention;
		}

		public RouteLocalizationConvention Convention { get; set; }

		public void Configure(MvcOptions options)
		{
			options.Conventions.Add(Convention);
		}
	}
}