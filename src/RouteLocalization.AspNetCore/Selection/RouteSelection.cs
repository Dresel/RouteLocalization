namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class RouteSelection
	{
		public ICollection<ActionModel> ActionModels { get; set; }

		public ControllerModel ControllerModel { get; set; }
	}
}