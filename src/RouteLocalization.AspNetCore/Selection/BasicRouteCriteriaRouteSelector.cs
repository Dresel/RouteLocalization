namespace RouteLocalization.AspNetCore.Selection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class BasicRouteCriteriaRouteSelector : IRouteSelector
	{
		public BasicRouteCriteriaRouteSelector(ILocalizer localizer)
		{
			Localizer = localizer;
		}

		public string Action { get; set; }

		public ICollection<Type> ActionArguments { get; set; }

		public string Controller { get; set; }

		public string ControllerNamespace { get; set; }

		public ILocalizer Localizer { get; set; }

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			Func<ControllerModel, ICollection<ActionModel>> actionSelector;

			if (!string.IsNullOrEmpty(Action))
			{
				actionSelector = controllerModel => controllerModel.Actions
					.Where(action => action.MatchesActionName(Action) && action.MatchesActionArguments(ActionArguments))
					.ToList();
			}
			else
			{
				actionSelector = controllerModel => null;
			}

			return applicationModel.Controllers
				.Where(controller => controller.MatchesController(Controller, ControllerNamespace) &&
					controller.IsOriginalModel(Localizer))
				.Select(controller => new RouteSelection()
				{
					ControllerModel = controller,
					ActionModels = actionSelector(controller)
				})
				.ToList();
		}
	}
}