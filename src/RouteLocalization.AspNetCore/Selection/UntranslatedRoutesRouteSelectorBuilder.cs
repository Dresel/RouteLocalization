namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class UntranslatedRoutesRouteSelectorBuilder : IRouteSelector
	{
		public string Culture { get; set; }

		public ILocalizer Localizer { get; set; }

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			bool HasNotCompletelyTranslatedControllerAttributeRoute(ControllerModel controller)
			{
				return controller.HasAttributeRoutes() && controller.TryGetLocalizedModelFor(Localizer, Culture)
					?.IsCompletelyTranslated(Localizer) != true;
			}

			bool IsNotCompletelyTranslatedAction(ActionModel action)
			{
				return action.HasAttributeRoutes() && action.TryGetLocalizedModelFor(Localizer, Culture)
					?.IsCompletelyTranslated(Localizer) != true;
			}

			bool HasNotCompletelyTranslatedActionAttributeRoute(ControllerModel controller)
			{
				return controller.Actions.Any(IsNotCompletelyTranslatedAction);
			}

			return applicationModel.Controllers.Where(controller => controller.IsOriginalModel(Localizer) &&
					(HasNotCompletelyTranslatedControllerAttributeRoute(controller) ||
						HasNotCompletelyTranslatedActionAttributeRoute(controller)))
				.Select(controller => new RouteSelection()
				{
					ControllerModel = controller,
					ActionModels = controller.Actions.Where(IsNotCompletelyTranslatedAction).ToList()
				})
				.ToList();
		}
	}
}