#if ASPNETWEBAPI
namespace RouteLocalization.Http.Routing
#else
namespace RouteLocalization.Mvc.Routing
#endif
{
	using System.Collections.Generic;
	using System.Linq;

#if ASPNETWEBAPI
	using System.Web.Http.Routing;
	using TActionDescriptor = System.Web.Http.Controllers.HttpActionDescriptor;
	using TControllerDescriptor = System.Web.Http.Controllers.HttpControllerDescriptor;
#else
	using System.Web.Mvc;
	using System.Web.Mvc.Routing;
	using TActionDescriptor = System.Web.Mvc.ActionDescriptor;
	using TControllerDescriptor = System.Web.Mvc.ControllerDescriptor;
#endif

	public class LocalizationDirectRouteProvider : IDirectRouteProvider
	{
		public LocalizationDirectRouteProvider(IDirectRouteProvider directRouteProvider)
		{
			DirectRouteProvider = directRouteProvider;
			LocalizationCollectionRoutes = new List<RouteEntry>();
		}

		public List<RouteEntry> LocalizationCollectionRoutes { get; set; }

		protected IDirectRouteProvider DirectRouteProvider { get; set; }

		public IReadOnlyList<RouteEntry> GetDirectRoutes(TControllerDescriptor controllerDescriptor,
			IReadOnlyList<TActionDescriptor> actionDescriptors, IInlineConstraintResolver constraintResolver)
		{
			IReadOnlyCollection<RouteEntry> controllerDirectRoutes = DirectRouteProvider.GetDirectRoutes(controllerDescriptor,
				actionDescriptors, constraintResolver);

			// Wrap routes with LocalizationCollectionRoute
			List<RouteEntry> routeEntries =
				controllerDirectRoutes.Select(x => new RouteEntry(x.Name, new LocalizationCollectionRoute(x.Route))).ToList();

			// Store routes for later processing
			LocalizationCollectionRoutes.AddRange(routeEntries);

			return routeEntries;
		}
	}
}