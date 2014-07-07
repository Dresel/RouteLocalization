namespace RouteLocalization.Http.Routing
{
	using System.Web.Http.Routing;

	public class LocalizationRoute : HttpRoute
	{
		public LocalizationRoute(IHttpRoute route, string culture)
			: base(
				route.RouteTemplate, new HttpRouteValueDictionary(route.Defaults), new HttpRouteValueDictionary(route.Constraints),
				new HttpRouteValueDictionary(route.DataTokens), route.Handler)
		{
			Culture = culture;
		}

		public string Culture { get; protected set; }

		public string LocalizedUrl
		{
			get
			{
				return RouteTemplate;
			}
		}
	}
}