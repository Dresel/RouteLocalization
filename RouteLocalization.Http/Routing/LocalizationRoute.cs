namespace RouteLocalization.Http.Routing
{
	using System.Net.Http;
	using System.Web.Http.Routing;

	public class LocalizationRoute : HttpRoute
	{
		public LocalizationRoute(string routeTemplate, HttpRouteValueDictionary defaults, HttpRouteValueDictionary constraints,
			HttpRouteValueDictionary dataTokens, HttpMessageHandler handler, string culture)
			: base(routeTemplate, defaults, constraints, dataTokens, handler)
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