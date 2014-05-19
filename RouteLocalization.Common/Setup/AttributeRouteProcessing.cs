#if ASPNETWEBAPI
namespace RouteLocalization.Http.Setup
#else
namespace RouteLocalization.Mvc.Setup
#endif
{
	public enum AttributeRouteProcessing
	{
		None,

		AddAsNeutralRoute,

		AddAsDefaultCultureRoute,

		AddAsNeutralAndDefaultCultureRoute,

		AddAsNeutralRouteAndReplaceByFirstTranslation
	}
}