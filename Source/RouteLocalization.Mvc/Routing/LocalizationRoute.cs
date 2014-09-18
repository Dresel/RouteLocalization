namespace RouteLocalization.Mvc.Routing
{
	using System.Web.Routing;

	public class LocalizationRoute : Route
	{
		public LocalizationRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
			RouteValueDictionary dataTokens, IRouteHandler routeHandler, string culture)
			: base(url, defaults, constraints, dataTokens, routeHandler)
		{
			Culture = culture;
		}

		public string Culture { get; protected set; }

		public string LocalizedUrl
		{
			get
			{
				return Url;
			}
		}
	}
}