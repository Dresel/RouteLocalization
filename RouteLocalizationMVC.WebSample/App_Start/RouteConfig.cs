namespace RouteLocalizationMVC.WebSample
{
	using System.Globalization;
	using System.Linq;
	using System.Web.Mvc;
	using System.Web.Routing;
	using RouteLocalizationMVC.Extensions;
	using RouteLocalizationMVC.Setup;
	using RouteLocalizationMVC.WebSample.Controllers;

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

			// Add accepted cultures
			Configuration.AcceptedCultures.Add("de");

			// Applies the default culture to original route if its translated
			Configuration.ApplyDefaultCultureToRootRoute = true;

			// Uncomment if you want the culture (en, de, ...) added to each translated route as route prefix
			// Configuration.AddCultureAsRoutePrefix = true;

			// Set LocalizationHttpModule to initialize ThreadCulture from Browser UserLanguages
			// Other options would be to load this from Cookie or Domain TLC (.com, .de, ...)
			LocalizationHttpModule.GetCultureFromHttpContextDelegate = httpContext =>
			{
				// Set default culture as fallback
				string cultureName = Configuration.DefaultCulture;

				if (httpContext.Request.UserLanguages != null)
				{
					// Get language from HTTP Header
					foreach (string userLanguage in httpContext.Request.UserLanguages.Select(x => x.Split(';').First()))
					{
						try
						{
							CultureInfo userCultureInfo = new CultureInfo(userLanguage);

							// We don't can / want to support all languages
							if (!Configuration.AcceptedCultures.Contains(userCultureInfo.Name.ToLower()))
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
			routes
				.ForCulture("de")
				.ForController<HomeController>()
					.ForAction(x => x.Index())
						.AddTranslation("Willkommen")
					.ForAction(x => x.Book())
						.AddTranslation("Buch/{chapter}/{page}");

			routes
				.ForCulture("de")
				.ForController<HomeWithRoutePrefixAttributeController>()
				.SetRoutePrefix("RoutePrefixDE")
					.ForAction(x => x.Index())
						.AddTranslation("Willkommen")
					.ForAction(x => x.Book())
						.AddTranslation("Buch/{chapter}/{page}");

			routes
				.ForCulture("de")
				.SetAreaPrefix("AreaPrefixDE")
				.ForController<HomeWithRouteAreaAttributeController>()
					.ForAction(x => x.Index())
						.AddTranslation("Willkommen")
					.ForAction(x => x.Book())
						.AddTranslation("Buch/{chapter}/{page}");
		}
	}
}