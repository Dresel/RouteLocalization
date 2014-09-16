namespace RouteLocalization.Mvc.Routing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Web;
	using System.Web.Routing;
	using RouteLocalization.Mvc.Extensions;

	public class LocalizationCollectionRoute : Route
	{
		public LocalizationCollectionRoute(Route route)
			: base(
				route.Url, new RouteValueDictionary(route.Defaults), new RouteValueDictionary(route.Constraints),
				new RouteValueDictionary(route.DataTokens), route.RouteHandler)
		{
			LocalizedRoutesContainer = new Dictionary<string, LocalizationRoute>(StringComparer.OrdinalIgnoreCase);
		}

		public ICollection<LocalizationRoute> LocalizedRoutes
		{
			get
			{
				return LocalizedRoutesContainer.Values;
			}
		}

		public LocalizationRoute NeutralRoute
		{
			get
			{
				return LocalizedRoutesContainer.ContainsKey(string.Empty) ? LocalizedRoutesContainer[string.Empty] : null;
			}

			protected set
			{
				LocalizedRoutesContainer[string.Empty] = value;
			}
		}

		protected IDictionary<string, LocalizationRoute> LocalizedRoutesContainer { get; set; }

		public void AddTranslation(string url, string culture)
		{
			LocalizedRoutesContainer[culture] = this.ToLocalizationRoute(url, culture);
		}

		public LocalizationRoute GetLocalizationRoute(HttpContextBase httpContext)
		{
			// Does not return the same as GetRouteData(httpContext).Route
			return LocalizedRoutesContainer.FirstOrDefault(x => x.Value.GetRouteData(httpContext) != null).Value;
		}

		public LocalizationRoute GetLocalizedRoute(string culture)
		{
			return LocalizedRoutesContainer.ContainsKey(culture) ? LocalizedRoutesContainer[culture] : null;
		}

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			return LocalizedRoutesContainer.Select(x => x.Value.GetRouteData(httpContext)).FirstOrDefault(x => x != null);
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

			// If specific path is requested, override culture and remove RouteValue
			if (values.ContainsKey("culture"))
			{
				currentCulture = (string)values["culture"];
			}

			RouteBase localizationRoute = GetLocalizedOrDefaultRoute(currentCulture);

			if (localizationRoute == null)
			{
				return null;
			}

			// Get translated route from child route
			VirtualPathData pathData = localizationRoute.GetVirtualPath(requestContext,
				new RouteValueDictionary(CopyAndRemoveFromValueDictionary(values)));

			return pathData;
		}

		public bool HasTranslationForCulture(string culture)
		{
			return LocalizedRoutesContainer.ContainsKey(culture);
		}

		public void RemoveTranslation(string culture)
		{
			LocalizedRoutesContainer.Remove(culture);
		}

		protected IDictionary<string, object> CopyAndRemoveFromValueDictionary(IDictionary<string, object> values)
		{
			Dictionary<string, object> routeValueDictionary = new Dictionary<string, object>(values,
				StringComparer.OrdinalIgnoreCase);

			routeValueDictionary.Remove("culture");

			return routeValueDictionary;
		}

		protected RouteBase GetLocalizedOrDefaultRoute(string currentCulture)
		{
			LocalizationRoute route;

			LocalizedRoutesContainer.TryGetValue(currentCulture, out route);

			return route ?? NeutralRoute;
		}
	}
}