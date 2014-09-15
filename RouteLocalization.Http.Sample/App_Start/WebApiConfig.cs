namespace RouteLocalization.Http.Sample
{
	using System.Collections.Generic;
	using System.Web.Http;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Setup;

	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// New workflow since 5.2
			// This provider wraps generated attribute routes with LocalizationCollectionRoute routes
			////LocalizationDirectRouteProvider provider
			////	= new LocalizationDirectRouteProvider(new DefaultDirectRouteProvider());

			// Web API routes
			// For less code preparation use the static provider stored in Localization class
			config.MapHttpAttributeRoutes(Localization.LocalizationDirectRouteProvider);

			const string defaultCulture = "en";
			ISet<string> acceptedCultures = new HashSet<string>() { defaultCulture, "de" };

			// Continue with localization configuration after Web API Routes configuration
			config.ContinueAfterPreviousInitialization(httpConfiguration =>
			{
				httpConfiguration.Routes.Localization(configuration =>
				{
					// Important: Set the route collection from LocalizationDirectRouteProvider if you specify your own
					//// configuration.LocalizationCollectionRoutes = provider.LocalizationCollectionRoutes;

					configuration.DefaultCulture = defaultCulture;
					configuration.AcceptedCultures = acceptedCultures;

					// Define how attribute routes should be processed:
					// * None: There will be no routes except the ones you explicitly define in Translate()
					// * AddAsNeutralRoute: Every attribute route will be added as neutral route
					// * AddAsDefaultCultureRoute: Every attribute route will be added as localized route for defined default culture
					// * AddAsNeutralAndDefaultCultureRoute: Every attribute route will be added as neutral route and
					//   as localized route for defined default culture
					// * AddAsNeutralRouteAndReplaceByFirstTranslation: Every attribute route will be added as neutral route first, but when
					//   you add a translation for a route, the neutral route will be removed
					configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute;

					// Uncomment if you do not want the culture (en, de, ...) added to each translated route as route prefix
					configuration.AddCultureAsRoutePrefix = true;
				}).TranslateInitialAttributeRoutes().Translate(localization =>
				{
					// Use extension methods if you want to separate route configuration
					localization.AddDefaultRoutesTranslation();

					// DefaultRoutes.cs
					////localization.ForCulture("de")
					////	.ForController<HomeController>()
					////	.ForAction(x => x.Index())
					////	.AddTranslation("Willkommen")
					////	.ForAction(x => x.Book(0, 0))
					////	.AddTranslation("Buch/{chapter}/{page}");

					////localization.ForCulture("de")
					////	.ForController<HomeWithRoutePrefixAttributeController>()
					////	.SetRoutePrefix("RoutePrefixDE")
					////		.ForAction(x => x.Index())
					////			.AddTranslation("Willkommen")
					////		.ForAction(x => x.Book())
					////			.AddTranslation("Buch/{chapter}/{page}");
				});

				// Optional
				// Setup CultureSensitiveMessageHandler
				// This Handler sets the Thread Culture and UICulture from http request message
				// Use predefined DetectCultureFromBrowserUserLanguages delegate or implement your own
				config.MessageHandlers.Add(new CultureSensitiveMessageHandler()
				{
					GetCultureFromHttpRequestMessageDelegate =
						Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture)
				});

				// Optional
				// Add culture sensitive action filter attribute
				// This sets the Culture and UICulture when a localized route is executed
				config.Filters.Add(new CultureSensitiveActionFilterAttribute());
			});
		}
	}
}