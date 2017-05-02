namespace RouteLocalization.AspNetCore
{
	using RouteLocalization.AspNetCore.Processor;
	using RouteLocalization.AspNetCore.Selection;

	public class RouteSelectorProcessorPair
	{
		public IRouteProcessor Processor { get; set; }

		public IRouteSelector Selector { get; set; }
	}
}