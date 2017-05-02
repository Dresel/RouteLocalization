namespace RouteLocalization.AspNetCore
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Options;

	// See https://github.com/aspnet/Mvc/issues/6214
	public class RouteLocalizationConventionConfigureOptions : IConfigureOptions<MvcOptions>
	{
		public RouteLocalizationConventionConfigureOptions(RouteLocalizationConvention convention)
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