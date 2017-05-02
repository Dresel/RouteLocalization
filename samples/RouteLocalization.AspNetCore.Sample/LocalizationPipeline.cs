namespace RouteLocalization.AspNetCore.Sample
{
	using Microsoft.AspNetCore.Builder;

	// See https://andrewlock.net/url-culture-provider-using-middleware-as-mvc-filter-in-asp-net-core-1-1-0/
	public class LocalizationPipeline
	{
		public void Configure(IApplicationBuilder app, RequestLocalizationOptions options)
		{
			app.UseRequestLocalization(options);
		}
	}
}