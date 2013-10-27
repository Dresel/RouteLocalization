namespace RouteLocalizationMVC.Extensions
{
	using System.Web.Mvc;
	using System.Web.Routing;

	public static class RouteExtension
	{
		public static TranslationRoute ToTranslationRoute(this Route route)
		{
			return new TranslationRoute(route.Url, new RouteValueDictionary(route.Defaults),
				new RouteValueDictionary(route.Constraints), new RouteValueDictionary(route.DataTokens),
				new CultureSensitiveRouteHandler(new MvcRouteHandler()));
		}
	}
}