#if ASPNETWEBAPI
namespace RouteLocalization.Http.Extensions
#else
namespace RouteLocalization.Mvc.Extensions
#endif
{
#if ASPNETWEBAPI
	using System.Net.Http;
	using RouteLocalization.Http.Routing;
	using TIRoute = System.Web.Http.Routing.IHttpRoute;
	using TRoute = System.Web.Http.Routing.HttpRoute;
	using TRouteValueDictionary = System.Web.Http.Routing.HttpRouteValueDictionary;
#else
	using System.Web.Routing;
	using RouteLocalization.Mvc.Routing;
	using TIRoute = System.Web.Routing.Route;
	using TRoute = System.Web.Routing.Route;
	using TRouteValueDictionary = System.Web.Routing.RouteValueDictionary;
#endif

	public static class RouteExtensions
	{
		public static LocalizationRoute ToLocalizationRoute(this TIRoute route, string url, string culture)
		{
			return
				new LocalizationRoute(
					new TRoute(url, new TRouteValueDictionary(route.Defaults), new TRouteValueDictionary(route.Constraints),
						new TRouteValueDictionary(route.DataTokens), route.RouteHandler()), culture);
		}

#if ASPNETWEBAPI
		public static HttpMessageHandler RouteHandler(this TIRoute route)
		{
			return route.Handler;
		}

		public static string Url(this TIRoute route)
		{
			return route.RouteTemplate;
		}
#else
		public static IRouteHandler RouteHandler(this TIRoute route)
		{
			return route.RouteHandler;
		}

		public static string Url(this TIRoute route)
		{
			return route.Url;
		}
#endif
	}
}