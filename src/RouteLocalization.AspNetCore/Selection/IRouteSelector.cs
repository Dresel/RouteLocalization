namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public interface IRouteSelector
	{
		ICollection<RouteSelection> Select(ApplicationModel applicationModel);
	}
}