namespace RouteLocalization.Mvc.Routing
{
	using System.Web;
	using System.Web.Routing;

	public class LocalizationRoute : RouteBase
	{
		public LocalizationRoute(Route route, string culture)
		{
			Route = route;
			Culture = culture;

			LocalizedUrl = route.Url;
		}

		public string Culture { get; protected set; }

		public string LocalizedUrl { get; protected set; }

		protected RouteBase Route { get; set; }

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			return Route.GetRouteData(httpContext);
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			return Route.GetVirtualPath(requestContext, values);
		}
	}
}