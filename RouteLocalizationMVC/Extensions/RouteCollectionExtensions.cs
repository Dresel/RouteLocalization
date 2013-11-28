namespace RouteLocalizationMVC.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Web.Routing;
	using RouteLocalizationMVC.Setup;

	public static class RouteCollectionExtensions
	{
		public static Route GetFirstUntranslatedRoute(this RouteCollection routeCollection, string culture, string controller,
			string action, string controllerNamespace, ICollection<Type> actionArguments)
		{
			List<Route> results =
				routeCollection.OfType<Route>().Where(
					x =>
						x.MatchesControllerAndAction(controller, action) && x.MatchesNamespace(controllerNamespace) &&
							x.HasNoTranslationForCulture(culture)).ToList();

			// Check if we can narrow down the selection with argument specification
			Route argumentRoute = results.FirstOrDefault(x => x.MatchesActionArguments(actionArguments));

			// If argumentRoute is null and actionArguments are mandatory
			if (argumentRoute == null && actionArguments != null)
			{
				return null;
			}

			// Return argumentRoute or first route without argument specification
			return argumentRoute ?? results.FirstOrDefault();
		}

		public static Route GetUntranslatedNamedRoute(this RouteCollection routeCollection, string culture, string namedRoute)
		{
			Route route = routeCollection[namedRoute] as Route;

			if (route == null)
			{
				return null;
			}

			if (!route.HasNoTranslationForCulture(culture))
			{
				return null;
			}

			return route;
		}

		public static Localization Localization(this RouteCollection routeCollection)
		{
			return new Localization() { RouteCollection = routeCollection };
		}

		public static Localization Localization(this RouteCollection routeCollection,
			Action<Configuration> configurationAction)
		{
			Configuration configuration = new Configuration();
			configurationAction.Invoke(configuration);

			return new Localization(configuration) { RouteCollection = routeCollection };
		}

		public static Localization Localization(this RouteCollection routeCollection, Configuration configuration)
		{
			return new Localization(configuration) { RouteCollection = routeCollection };
		}

		public static Dictionary<string, RouteBase> NamedMap(this RouteCollection routeCollection)
		{
			FieldInfo fieldInfo = routeCollection.GetType().GetField("_namedMap", BindingFlags.NonPublic | BindingFlags.Instance);

			return (Dictionary<string, RouteBase>)fieldInfo.GetValue(routeCollection);
		}

		private static bool HasNoTranslationForCulture(this Route route, string culture)
		{
			TranslationRoute translationRoute = route as TranslationRoute;

			if (translationRoute == null)
			{
				return true;
			}

			return translationRoute.IsRoot && translationRoute.Culture != culture && !translationRoute.TranslatedRoutes.ContainsKey(culture);
		}

		private static bool MatchesActionArguments(this Route route, ICollection<Type> actionArguments)
		{
			if (actionArguments == null)
			{
				return true;
			}

			MethodInfo methodInfo = (route.DataTokens["TargetActionMethod"] as MethodInfo);

			if (methodInfo != null)
			{
				ParameterInfo[] parameterInfos = methodInfo.GetParameters();

				if (parameterInfos.Count() != actionArguments.Count)
				{
					return false;
				}

				for (int i = 0; i < actionArguments.Count; i++)
				{
					if (actionArguments.ElementAt(i) != parameterInfos[i].ParameterType)
					{
						return false;
					}
				}
			}

			return true;
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