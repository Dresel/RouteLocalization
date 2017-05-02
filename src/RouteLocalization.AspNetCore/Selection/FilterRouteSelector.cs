namespace RouteLocalization.AspNetCore.Selection
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class FilterRouteSelector : IRouteSelector
	{
		public FilterRouteSelector(IRouteSelector routeSelector)
		{
			ParentRouteSelector = routeSelector;
		}

		public string Action { get; set; }

		public ICollection<Type> ActionArguments { get; set; }

		public string Controller { get; set; }

		public string ControllerNamespace { get; set; }

		public IRouteSelector ParentRouteSelector { get; set; }

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			ICollection<RouteSelection> routeSelections = ParentRouteSelector.Select(applicationModel);

			if (string.IsNullOrEmpty(Action))
			{
				return routeSelections
					.Where(selection => !selection.ControllerModel.MatchesController(Controller, ControllerNamespace))
					.ToList();
			}

			ICollection<RouteSelection> routeSelectionsNew = new List<RouteSelection>();

			foreach (RouteSelection routeSelection in routeSelections)
			{
				if (!routeSelection.ControllerModel.MatchesController(Controller, ControllerNamespace))
				{
					routeSelectionsNew.Add(routeSelection);
				}

				bool hadActions = routeSelection.ActionModels.Any();

				routeSelection.ActionModels = routeSelection.ActionModels
					.Where(action => !(action.MatchesActionName(Action) && action.MatchesActionArguments(ActionArguments)))
					.ToList();

				if (!routeSelection.ActionModels.Any() && hadActions)
				{
					continue;
				}

				routeSelectionsNew.Add(routeSelection);
			}

			return routeSelectionsNew;
		}
	}
}