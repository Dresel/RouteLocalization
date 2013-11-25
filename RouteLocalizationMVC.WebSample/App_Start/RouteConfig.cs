namespace RouteLocalizationMVC.WebSample
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Web.Mvc;
	using System.Web.Routing;
	using RouteLocalizationMVC.Extensions;
	using RouteLocalizationMVC.Setup;
	using RouteLocalizationMVC.WebSample.App_Start;

	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Map routes via attributes
			routes.MapMvcAttributeRoutes();

			// At the beginning of a request, the GetCultureFromHttpContextDelegate of LocalizationHttpModule is called to initialise thread culture.

			// If a culture specific TranslationRoute is requested, the thread culture gets overridden by CultureSensitiveRouteHandler.

			// Every translated route is replaced by a TranslationRoute. The overridden GetVirtualPath uses the current culture for route generation.
			// This ensures that for example german routes are used when the thread culture is german.

			// Define values here so they can used for the LocalizationHttpModule too
			string defaultCulture = "en";
			HashSet<string> acceptedCultures = new HashSet<string>() { "en", "de" };

			// Set LocalizationHttpModule to initialize ThreadCulture from Browser UserLanguages
			// Other options would be to load this from Cookie or Domain TLC (.com, .de, ...)
			LocalizationHttpModule.GetCultureFromHttpContextDelegate = httpContext =>
			{
				// Set default culture as fallback
				string cultureName = defaultCulture;

				if (httpContext.Request.UserLanguages != null)
				{
					// Get language from HTTP Header
					foreach (string userLanguage in httpContext.Request.UserLanguages.Select(x => x.Split(';').First()))
					{
						try
						{
							CultureInfo userCultureInfo = new CultureInfo(userLanguage);

							// We don't can / want to support all languages
							if (!acceptedCultures.Contains(userCultureInfo.Name.ToLower()))
							{
								continue;
							}

							// Culture found that is supported
							cultureName = userCultureInfo.Name.ToLower();
							break;
						}
						catch
						{
							// Ignore invalid cultures
							continue;
						}
					}
				}

				// Return accepted culture
				return new CultureInfo(cultureName);
			};

			// Add translations
			// You can translate every specific route that contains default Controller and Action (which MapMvcAttributeRoutes does)
			routes.Localization(configuration =>
			{
				configuration.DefaultCulture = defaultCulture;
				configuration.AcceptedCultures = acceptedCultures;

				// Use existing attribute routes as translated route with default culture (english)
				// Other options would be
				// * Replace the existing attribute routes with the translated route
				// * Do not process attribute route and let it be a "neutral" route
				configuration.RootTranslationProcessing = RootTranslationProcessing.ReplaceRouteByTranslatedRoute;

				// Uncomment if you want the culture (en, de, ...) added to each translated route as route prefix
				configuration.AddCultureAsRoutePrefix = true;
			}).Translate(localization =>
			{
				// Use extension methods if you want to separate route configuration
				localization.AddDefaultRoutesTranslation();
				localization.AddAreaRoutesTranslation();

				// DefaultRoutes.cs
				//localization.ForCulture("de")
				//	.ForController<HomeController>()
				//	.ForAction(x => x.Index())
				//		.AddTranslation("Willkommen")
				//	.ForAction(x => x.Book())
				//		.AddTranslation("Buch/{chapter}/{page}");

				//localization.ForCulture("de")
				//	.ForController<HomeWithRoutePrefixAttributeController>()
				//	.SetRoutePrefix("RoutePrefixDE")
				//		.ForAction(x => x.Index())
				//			.AddTranslation("Willkommen")
				//		.ForAction(x => x.Book())
				//			.AddTranslation("Buch/{chapter}/{page}");

				//localization.ForCulture("de")
				//	.SetAreaPrefix("AreaPrefixDE")
				//	.ForController<HomeWithRouteAreaAttributeController>()
				//		.ForAction(x => x.Index())
				//			.AddTranslation("Willkommen")
				//		.ForAction(x => x.Book())
				//			.AddTranslation("Buch/{chapter}/{page}");

				// AreaRoutes.cs
				//localization.ForCulture("de")
				//	.SetAreaPrefix("Area")
				//	.ForController<Areas.Area.Controllers.HomeController>()
				//	.ForAction(x => x.Index())
				//		.AddTranslation("Willkommen")
				//	.ForAction(x => x.Book())
				//		.AddTranslation("Buch/{chapter}/{page}");
			});
		}
	}
}