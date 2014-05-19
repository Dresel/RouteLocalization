namespace RouteLocalization.Http.Routing
{
	using System.Collections.Generic;
	using System.Net.Http;
	using System.Web.Http.Routing;

	public class LocalizationRoute : IHttpRoute
	{
		public LocalizationRoute(IHttpRoute route, string culture)
		{
			Route = route;
			Culture = culture;
		}

		public IDictionary<string, object> Constraints
		{
			get
			{
				return Route.Constraints;
			}
		}

		public string Culture { get; protected set; }

		public IDictionary<string, object> DataTokens
		{
			get
			{
				return Route.DataTokens;
			}
		}

		public IDictionary<string, object> Defaults
		{
			get
			{
				return Route.Defaults;
			}
		}

		public HttpMessageHandler Handler
		{
			get
			{
				return Route.Handler;
			}
		}

		public string LocalizedUrl
		{
			get
			{
				return RouteTemplate;
			}
		}

		public string RouteTemplate
		{
			get
			{
				return Route.RouteTemplate;
			}
		}

		protected IHttpRoute Route { get; set; }

		public IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
		{
			return Route.GetRouteData(virtualPathRoot, request);
		}

		public IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
		{
			return Route.GetVirtualPath(request, values);
		}
	}
}