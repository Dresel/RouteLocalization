using RouteLocalizationMVC.Setup;

namespace RouteLocalizationMVC.Extensions
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Web.Routing;

	public static class RouteCollectionExtensions
	{
		public static RouteTranslator Localization(this RouteCollection collection)
		{
			return new RouteTranslator() {RouteCollection = collection};
		}

		public static RouteTranslator Localization(this RouteCollection collection, Action<Configuration> configurationAction)
		{
			Configuration configuration = new Configuration();
			configurationAction.Invoke(configuration);

			return new RouteTranslator(configuration) { RouteCollection = collection };
		}

		public static RouteTranslator Localization(this RouteCollection collection, Configuration configuration)
		{
			return new RouteTranslator(configuration) { RouteCollection = collection };
		}

		public static Route GetFirstUntranslatedRoute(this RouteCollection routeCollection, string culture, string controller,
			string action, string controllerNamespace)
		{
			return
				routeCollection.OfType<Route>().FirstOrDefault(
					x =>
						x.MatchesControllerAndAction(controller, action) && x.MatchesNamespace(controllerNamespace) &&
							x.HasNoTranslationForCulture(culture));
		}

		//public static RouteTranslator SetAreaPrefix(this RouteCollection collection, string areaPrefix)
		//{
		//	RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

		//	return routeTranslator.SetAreaPrefix(areaPrefix);
		//}

		//public static RouteTranslator SetRoutePrefix(this RouteCollection collection, string routePrefix)
		//{
		//	RouteTranslator routeTranslator = new RouteTranslator() { RouteCollection = collection };

		//	return routeTranslator.SetRoutePrefix(routePrefix);
		//}

		private static bool HasNoTranslationForCulture(this Route route, string culture)
		{
			TranslationRoute translationRoute = route as TranslationRoute;

			if (translationRoute == null)
			{
				return true;
			}

			return translationRoute.IsRoot && !translationRoute.TranslatedRoutes.ContainsKey(culture);
		}

		private static bool MatchesControllerAndAction(this Route route, string controller, string action)
		{
			return route.Defaults != null && (string)route.Defaults["controller"] == controller &&
				(string)route.Defaults["action"] == action;
		}

		private static bool MatchesNamespace(this Route route, string controllerNamespace)
		{
			if (string.IsNullOrEmpty(controllerNamespace))
			{
				return true;
			}

			MethodInfo methodInfo = (route.DataTokens["TargetActionMethod"] as MethodInfo);

			if (methodInfo != null)
			{
				return methodInfo.DeclaringType.Namespace == controllerNamespace;
			}

			string[] namespaces = (route.DataTokens["Namespaces"] as string[]);

			if (namespaces != null)
			{
				return namespaces.Contains(controllerNamespace);
			}

			return true;
		}
	}
}