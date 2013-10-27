namespace RouteLocalizationMVC.WebSample
{
	using System.Web;
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

			// At the beginning of a request, LocalizationHttpModule checks browser user languages,
			// and sets the thread culture to this language if it is in AcceptedCultures.
			// If its not, thread culture is set to Configuration.DefaultCulture.

			// If a culture specific TranslationRoute is requested, the thread culture gets overridden by CultureSensitiveRouteHandler.

			// The current culture is used for TranslationRoute.GetVirtualPath to force culture specific route generation.
			// This is useful when UrlHelper.Route, RedirectToRoute or similiar functions are called

			// Add accepted cultures
			Configuration.AcceptedCultures.Add("de");

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