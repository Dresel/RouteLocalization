namespace RouteLocalization.AspNetCore.Processor
{
	using System.Collections.Generic;
	using RouteLocalization.AspNetCore.Selection;

	public interface IRouteProcessor
	{
		void Process(ICollection<RouteSelection> routeSelections);
	}
}