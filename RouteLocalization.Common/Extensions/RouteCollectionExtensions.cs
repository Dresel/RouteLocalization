#if ASPNETWEBAPI
namespace RouteLocalization.Http.Extensions
#else
namespace RouteLocalization.Mvc.Extensions
#endif
{
	using System;

#if ASPNETWEBAPI
	using RouteLocalization.Http.Setup;
	using TRouteCollection = System.Web.Http.HttpRouteCollection;
#else
	using RouteLocalization.Mvc.Setup;
	using TRouteCollection = System.Web.Routing.RouteCollection;
#endif

	public static class RouteCollectionExtensions
	{
		public static Localization Localization(this TRouteCollection routeCollection)
		{
			return routeCollection.Localization(new Configuration());
		}

		public static Localization Localization(this TRouteCollection routeCollection,
			Action<Configuration> configurationAction)
		{
			Configuration configuration = new Configuration();
			configurationAction.Invoke(configuration);

			return new Localization(configuration);
		}

		public static Localization Localization(this TRouteCollection routeCollection, Configuration configuration)
		{
			return new Localization(configuration);
		}
	}
}