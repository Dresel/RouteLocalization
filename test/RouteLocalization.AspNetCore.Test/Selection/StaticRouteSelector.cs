namespace RouteLocalization.AspNetCore.Test.Selection
{
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using RouteLocalization.AspNetCore.Selection;

	public class StaticRouteSelector : IRouteSelector
	{
		public ICollection<RouteSelection> RouteSelection { get; set; } = new List<RouteSelection>();

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			return RouteSelection;
		}
	}
}