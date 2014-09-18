#if ASPNETWEBAPI
namespace RouteLocalization.Http.Routing
#else
namespace RouteLocalization.Mvc.Routing
#endif
{
#if ASPNETWEBAPI
	// Copied from System.Web.Http.Routing.RouteDataTokenKeys
	public static class RouteDataTokenKeys
	{
		// Used to provide the action descriptors to consider for attribute routing
		public const string Actions = "actions";

		// Used to indicate that a route is a controller-level attribute route
		public const string Controller = "controller";
	}
#else
	// Copied from System.Web.Mvc.Routing.RouteDataTokenKeys
	public class RouteDataTokenKeys
	{
		// Used to provide the action descriptors to consider for attribute routing
		public const string Actions = "MS_DirectRouteActions";

		// Used to prioritize routes to actions for link generation
		public const string TargetIsAction = "MS_DirectRouteTargetIsAction";
	}
#endif
}