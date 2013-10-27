namespace RouteLocalizationMVC.Extensions
{
	using System.Linq;
	using System.Web.Routing;

	public static class RouteCollectionExtensions
	{
		public static RouteTranslator AddTranslation(this RouteCollection collection, string url, string culture, Route route)
		{
			RouteTranslator translator = new RouteTranslator() { RouteCollection = collection };

			return translator.AddTranslation(url, culture, route);
		}

		public static RouteTranslator AddTranslation(this RouteCollection collection, string url, string culture,
			string controller, string action)
		{
			RouteTranslator translator = new RouteTranslator() { RouteCollection = collection };

			return translator.AddTranslation(url, culture, controller, action);
		}

		public static RouteTranslator ForController(this RouteCollection collection, string controller)
		{
			RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

			return routeTranslator.ForController(controller);
		}

		public static RouteTranslator<T> ForController<T>(this RouteCollection collection)
		{
			RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

			return routeTranslator.ForController<T>();
		}

		public static RouteTranslator ForCulture(this RouteCollection collection, string culture)
		{
			RouteTranslator translator = new RouteTranslator() { RouteCollection = collection };

			return translator.ForCulture(culture);
		}

		public static Route GetFirstUntranslatedRoute(this RouteCollection routeCollection, string culture, string controller,
			string action)
		{
			return
				routeCollection.OfType<Route>().FirstOrDefault(
					x =>
						x.Defaults != null && (string)x.Defaults["controller"] == controller && (string)x.Defaults["action"] == action &&
							(!(x is TranslationRoute) ||
								(((TranslationRoute)x).IsRoot && !((TranslationRoute)x).TranslatedRoutes.ContainsKey(culture))));
		}

		public static RouteTranslator SetAreaPrefix(this RouteCollection collection, string areaPrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

			return routeTranslator.SetAreaPrefix(areaPrefix);
		}

		public static RouteTranslator SetRoutePrefix(this RouteCollection collection, string routePrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

			return routeTranslator.SetRoutePrefix(routePrefix);
		}
	}
}