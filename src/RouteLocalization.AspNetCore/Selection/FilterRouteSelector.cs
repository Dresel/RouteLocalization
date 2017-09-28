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

		public ICollection<string> Cultures { get; set; }

		public bool FilterControllerOrActionWhenNoTranslatedRouteLeft { get; set; }

		public bool FilterControllerOrActionWhenNoUntranslatedRouteLeft { get; set; }

		public ILocalizer Localizer { get; set; }

		public IRouteSelector ParentRouteSelector { get; set; }

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			ICollection<RouteSelection> routeSelections = ParentRouteSelector.Select(applicationModel);

			// Filter on controller level only
			if (string.IsNullOrEmpty(Action))
			{
				return routeSelections
					.Where(selection => !selection.ControllerModel.MatchesController(Controller, ControllerNamespace)).ToList();
			}

			// Filter actions on controller level
			routeSelections = routeSelections.Select(selection =>
			{
				if (selection.ControllerModel.MatchesController(Controller, ControllerNamespace))
				{
					selection.ActionModels = selection.ActionModels
						.Where(action => !(action.MatchesActionName(Action) && action.MatchesActionArguments(ActionArguments))).ToList();
				}

				return selection;
			}).ToList();

			if (FilterControllerOrActionWhenNoTranslatedRouteLeft)
			{
				return routeSelections.Where(selection => selection.ControllerModel.IsOriginalModel(Localizer) &&
					(TranslatedRoutesRouteSelector.HasPartiallyTranslatedControllerOnlyAttributeRoute(selection.ControllerModel, selection.ActionModels,
							Cultures, Localizer) ||
						TranslatedRoutesRouteSelector.HasPartiallyTranslatedActionAttributeRoutes(selection.ActionModels, Cultures,
							Localizer))).Select(selection => new RouteSelection()
				{
					ControllerModel = selection.ControllerModel,
					ActionModels = selection.ActionModels
						.Where(action => TranslatedRoutesRouteSelector.IsPartiallyTranslatedAction(action, Cultures, Localizer)).ToList()
				}).ToList();
			}

			if (FilterControllerOrActionWhenNoUntranslatedRouteLeft)
			{
				return routeSelections.Where(selection => selection.ControllerModel.IsOriginalModel(Localizer) &&
					(UntranslatedRoutesRouteSelector.HasNotCompletelyTranslatedControllerAttributeRoute(selection.ControllerModel,
							Cultures.Single(), Localizer) ||
						UntranslatedRoutesRouteSelector.HasNotCompletelyTranslatedActionAttributeRoute(selection.ActionModels,
							Cultures.Single(), Localizer))).Select(selection => new RouteSelection()
				{
					ControllerModel = selection.ControllerModel,
					ActionModels = selection.ActionModels
						.Where(action => UntranslatedRoutesRouteSelector.IsNotCompletelyTranslatedAction(action, Cultures.Single(),
							Localizer)).ToList()
				}).ToList();
			}

			return routeSelections;
		}
	}
}