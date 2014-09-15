namespace RouteLocalization.Http.Routing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Http;
	using System.Threading;
	using System.Web.Http.Routing;
	using RouteLocalization.Http.Extensions;

	public class LocalizationCollectionRoute : IHttpRoute
	{
		public LocalizationCollectionRoute(IHttpRoute httpRoute)
		{
			HttpRoute = httpRoute;

			LocalizedRoutesContainer = new Dictionary<string, LocalizationRoute>(StringComparer.OrdinalIgnoreCase);
		}

		public IDictionary<string, object> Constraints { get; set; }

		public IDictionary<string, object> DataTokens
		{
			get
			{
				return HttpRoute.DataTokens;
			}
		}

		public IDictionary<string, object> Defaults
		{
			get
			{
				return HttpRoute.Defaults;
			}
		}

		public HttpMessageHandler Handler
		{
			get
			{
				return HttpRoute.Handler;
			}
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

		public string RouteTemplate
		{
			get
			{
				return HttpRoute.RouteTemplate;
			}
		}

		protected IHttpRoute HttpRoute { get; set; }

		protected IDictionary<string, LocalizationRoute> LocalizedRoutesContainer { get; set; }

		public void AddTranslation(string url, string culture)
		{
			LocalizedRoutesContainer[culture] = this.ToLocalizationRoute(url, culture);
		}

		public LocalizationRoute GetLocalizationRoute(string virtualPathRoot, HttpRequestMessage request)
		{
			// Does not return the same as GetRouteData(httpContext).Route
			return LocalizedRoutesContainer.FirstOrDefault(x => x.Value.GetRouteData(virtualPathRoot, request) != null).Value;
		}

		public LocalizationRoute GetLocalizedRoute(string culture)
		{
			return LocalizedRoutesContainer.ContainsKey(culture) ? LocalizedRoutesContainer[culture] : null;
		}

		public IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
		{
			return
				LocalizedRoutesContainer.Select(x => x.Value.GetRouteData(virtualPathRoot, request)).FirstOrDefault(x => x != null);
		}

		public IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
		{
			IHttpRoute localizationRoute = GetLocalizedOrDefaultRoute(values);

			if (localizationRoute == null)
			{
				return null;
			}

			// Get translated route from child route
			IHttpVirtualPathData pathData = localizationRoute.GetVirtualPath(request, values);

			if (pathData != null)
			{
				pathData = localizationRoute.GetVirtualPath(request, CopyAndRemoveFromValueDictionary(values));
			}

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

		protected IHttpRoute GetLocalizedOrDefaultRoute(IDictionary<string, object> values)
		{
			string currentCulture = Thread.CurrentThread.CurrentUICulture.Name;

			// If specific path is requested, override culture and remove RouteValue
			if (values.ContainsKey("culture"))
			{
				currentCulture = (string)values["culture"];
			}

			LocalizationRoute route;

			LocalizedRoutesContainer.TryGetValue(currentCulture, out route);

			return route ?? NeutralRoute;
		}
	}
}