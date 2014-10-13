#if ASPNETWEBAPI
namespace RouteLocalization.Http.Extensions
#else
namespace RouteLocalization.Mvc.Extensions
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

#if ASPNETWEBAPI
	using System.Web.Http.Routing;
	using RouteLocalization.Http.Routing;
	using TActionDescriptor = System.Web.Http.Controllers.HttpActionDescriptor;
	using TControllerDescriptor = System.Web.Http.Controllers.HttpControllerDescriptor;
	using TParameterDescriptor = System.Web.Http.Controllers.HttpParameterDescriptor;
	using TRoute = System.Web.Http.Routing.IHttpRoute;
#else
	using System.Web.Mvc.Routing;
	using RouteLocalization.Mvc.Routing;
	using TActionDescriptor = System.Web.Mvc.ActionDescriptor;
	using TControllerDescriptor = System.Web.Mvc.ControllerDescriptor;
	using TParameterDescriptor = System.Web.Mvc.ParameterDescriptor;
	using TRoute = System.Web.Routing.Route;
#endif

	public static class RouteEntryCollectionExtensions
	{
		public static LocalizationCollectionRoute GetNamedRoute(this ICollection<RouteEntry> routeEntries, string culture,
			string namedRoute)
		{
			RouteEntry routeEntry = routeEntries.SingleOrDefault(x => x.Name == namedRoute);

			if (routeEntry == null)
			{
				return null;
			}

			return (LocalizationCollectionRoute)routeEntry.Route;
		}

		public static ICollection<LocalizationCollectionRoute> GetRoutes(this ICollection<RouteEntry> routeEntries,
			string culture, string controller, string action, string controllerNamespace, ICollection<Type> actionArguments)
		{
			return
				routeEntries.ToLocalizationCollectionRoutes()
					.WhereMatchesDescription(controller, action, controllerNamespace, actionArguments)
					.ToList();
		}

		public static string ToRoutesString(this ICollection<RouteEntry> routeEntries)
		{
			return string.Join(Environment.NewLine,
				routeEntries.Select(x => (LocalizationCollectionRoute)x.Route)
					.SelectMany(x => x.LocalizedRoutes.Select(route => string.Format("{0} ({1})", route.LocalizedUrl, route.Culture))));
		}

		private static bool MatchesActionArguments(this TRoute route, ICollection<Type> actionArguments)
		{
			if (actionArguments == null)
			{
				return true;
			}

			// When specifying arguments, it must be an action level attribute route with a single action descriptor
			TActionDescriptor actionDescriptor = ((TActionDescriptor[])route.DataTokens[RouteDataTokenKeys.Actions]).Single();

			ICollection<TParameterDescriptor> parameterInfos = actionDescriptor.GetParameters();

			if (parameterInfos.Count() != actionArguments.Count)
			{
				return false;
			}

			for (int i = 0; i < actionArguments.Count; i++)
			{
				if (actionArguments.ElementAt(i) != parameterInfos.ElementAt(i).ParameterType)
				{
					return false;
				}
			}

			return true;
		}

		private static bool MatchesControllerAndAction(this TRoute route, string controller, string action)
		{
#if ASPNETWEBAPI
			// Controller is only set on controller level attributes
			if (route.DataTokens.ContainsKey(RouteDataTokenKeys.Controller))
			{
				return ((TControllerDescriptor)route.DataTokens[RouteDataTokenKeys.Controller]).ControllerName == controller &&
					string.IsNullOrEmpty(action);
			}
#else
	// Controller / Action level attributes are distinguished by TargetIsAction
			if (((bool?)route.DataTokens[RouteDataTokenKeys.TargetIsAction]) != true)
			{
				return
					((TActionDescriptor[])route.DataTokens[RouteDataTokenKeys.Actions]).First().ControllerDescriptor.ControllerName ==
						controller && string.IsNullOrEmpty(action);
			}
#endif

			TActionDescriptor httpActionDescriptor = ((TActionDescriptor[])route.DataTokens[RouteDataTokenKeys.Actions]).Single();
			return httpActionDescriptor.ControllerDescriptor.ControllerName == controller &&
				httpActionDescriptor.ActionName == action;
		}

		private static bool MatchesNamespace(this TRoute route, string controllerNamespace)
		{
			if (string.IsNullOrEmpty(controllerNamespace))
			{
				return true;
			}

			TActionDescriptor[] actionDescriptors = (TActionDescriptor[])route.DataTokens[RouteDataTokenKeys.Actions];

			TControllerDescriptor controllerDescriptor = actionDescriptors.First().ControllerDescriptor;

			return controllerDescriptor.ControllerType.Namespace == controllerNamespace;
		}

		private static IEnumerable<LocalizationCollectionRoute> ToLocalizationCollectionRoutes(
			this ICollection<RouteEntry> routeEntries)
		{
			return routeEntries.Select(x => (LocalizationCollectionRoute)x.Route);
		}

		private static IEnumerable<LocalizationCollectionRoute> WhereMatchesDescription(
			this IEnumerable<LocalizationCollectionRoute> localizationCollectionRoutes, string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			return
				localizationCollectionRoutes.Where(
					x =>
						x.MatchesControllerAndAction(controller, action) && x.MatchesNamespace(controllerNamespace) &&
							(actionArguments == null || x.MatchesActionArguments(actionArguments)));
		}
	}
}