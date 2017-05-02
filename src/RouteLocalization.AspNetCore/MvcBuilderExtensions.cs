namespace RouteLocalization.AspNetCore
{
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;

	public static class MvcBuilderExtensions
	{
		public static IMvcBuilder AddRouteLocalization(this IMvcBuilder mvcBuilder, Action<RouteTranslator> setupAction)
		{
			mvcBuilder.Services.AddSingleton<RouteTranslationConfiguration>();
			mvcBuilder.Services.AddSingleton<RouteTranslator>();
			mvcBuilder.Services.AddSingleton<RouteLocalizationConvention>();
			mvcBuilder.Services.AddSingleton<IConfigureOptions<MvcOptions>, RouteLocalizationConventionConfigureOptions>();

			mvcBuilder.Services.Configure<RouteTranslatorOptions>(options => options.RouteTranslatorAction = setupAction);

			return mvcBuilder;
		}
	}
}