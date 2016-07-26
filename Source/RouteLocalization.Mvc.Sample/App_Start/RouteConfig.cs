namespace RouteLocalization.Mvc.Sample
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using System.Web.Routing;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;

	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// New workflow since 5.2
			// This provider wraps generated attribute routes with LocalizationCollectionRoute routes
			////LocalizationDirectRouteProvider provider
			////	= new LocalizationDirectRouteProvider(new DefaultDirectRouteProvider());

			// For less code preparation use the static provider stored in Localization class
			routes.MapMvcAttributeRoutes(Localization.LocalizationDirectRouteProvider);

			const string defaultCulture = "us";
			ISet<string> englishCultures = new HashSet<string>() { defaultCulture, "gb" };
			ISet<string> acceptedCultures = new HashSet<string>(englishCultures) { "de" };

			// Add translations
			// You can translate every specific route that contains default Controller and Action (which MapMvcAttributeRoutes does)
			routes.Localization(configuration =>
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
				configuration.AttributeRouteProcessing = AttributeRouteProcessing.AddAsNeutralRoute;

				// Uncomment if you do not want the culture (en, de, ...) added to each translated route as route prefix
				configuration.AddCultureAsRoutePrefix = true;

				configuration.AddTranslationToSimiliarUrls = true;
			}).TranslateInitialAttributeRoutes().Translate(localization =>
			{
				// Add english culture routes without translation
				englishCultures.ToList().ForEach(culture =>
				{
					// For every collected attribute route
					localization.Configuration.LocalizationCollectionRoutes.ForEach(route =>
					{
						// Add translation for this culture with same route as defined in route attribute
						localization.AddTranslation(route.Route.Url(), culture, (LocalizationCollectionRoute)route.Route);
					});
				});

				// Use extension methods if you want to separate route configuration
				localization.AddDefaultRoutesTranslation();
				localization.AddAreaRoutesTranslation();

				// DefaultRoutes.cs
				////localization.ForCulture("de")
				////	.ForController<HomeController>()
				////	.ForAction(x => x.Index())
				////		.AddTranslation("Willkommen")
				////	.ForAction(x => x.Book())
				////		.AddTranslation("Buch/{chapter}/{page}");

				////localization.ForCulture("de")
				////	.ForController<HomeWithRoutePrefixAttributeController>()
				////	.SetRoutePrefix("RoutePrefixDE")
				////		.ForAction(x => x.Index())
				////			.AddTranslation("Willkommen")
				////		.ForAction(x => x.Book())
				////			.AddTranslation("Buch/{chapter}/{page}");

				////localization.ForCulture("de")
				////	.SetAreaPrefix("AreaPrefixDE")
				////	.ForController<HomeWithRouteAreaAttributeController>()
				////		.ForAction(x => x.Index())
				////			.AddTranslation("Willkommen")
				////		.ForAction(x => x.Book())
				////			.AddTranslation("Buch/{chapter}/{page}");

				// AreaRoutes.cs
				////localization.ForCulture("de")
				////	.SetAreaPrefix("Area")
				////	.ForController<Areas.Area.Controllers.HomeController>()
				////	.ForAction(x => x.Index())
				////		.AddTranslation("Willkommen")
				////	.ForAction(x => x.Book())
				////		.AddTranslation("Buch/{chapter}/{page}");
			});

			// Optional
			// Setup CultureSensitiveHttpModule
			// This Module sets the Thread Culture and UICulture from http context
			// Use predefined DetectCultureFromBrowserUserLanguages delegate or implement your own
			CultureSensitiveHttpModule.GetCultureFromHttpContextDelegate =
				Localization.DetectCultureFromBrowserUserLanguages(acceptedCultures, defaultCulture);

			// Optional
			// Add culture sensitive action filter attribute
			// This sets the Culture and UICulture when a localized route is executed
			GlobalFilters.Filters.Add(new CultureSensitiveActionFilterAttribute()
			{
				// Set this options only if you want to support detection of region dependent cultures
				// Supports this use case: https://github.com/Dresel/RouteLocalization/issues/38#issuecomment-70999613
				AcceptedCultures = acceptedCultures,
				TryToPreserverBrowserRegionCulture = true
			});
		}
	}
}