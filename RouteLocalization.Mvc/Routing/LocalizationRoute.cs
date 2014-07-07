namespace RouteLocalization.Mvc.Routing
{
	using System.Web.Routing;

	public class LocalizationRoute : Route
	{
		public LocalizationRoute(Route route, string culture)
			: base(
				route.Url, new RouteValueDictionary(route.Defaults), new RouteValueDictionary(route.Constraints),
				new RouteValueDictionary(route.DataTokens), route.RouteHandler)
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