namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class TranslatedRoutesRouteSelector : IRouteSelector
	{
		public ICollection<string> Cultures { get; set; }

		public ILocalizer Localizer { get; set; }

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			bool HasPartiallyTranslatedControllerOnlyAttributeRoute(ControllerModel controller)
			{
				return controller.HasAttributeRoutes() && !controller.Actions.Any(action => action.HasAttributeRoutes()) &&
					Cultures.All(culture => controller.TryGetLocalizedModelFor(Localizer, culture)?.IsPartiallyTranslated(Localizer) ==
						true);
			}

			bool IsPartiallyTranslatedAction(ActionModel action)
			{
				return action.HasAttributeRoutes() && Cultures.All(
					culture => action.TryGetLocalizedModelFor(Localizer, culture)?.IsPartiallyTranslated(Localizer) == true);
			}

			bool HasPartiallyTranslatedActionAttributeRoutes(ControllerModel controller)
			{
				return controller.Actions.Any(IsPartiallyTranslatedAction);
			}

			return applicationModel.Controllers.Where(controller => controller.IsOriginalModel(Localizer) &&
					(HasPartiallyTranslatedControllerOnlyAttributeRoute(controller) ||
						HasPartiallyTranslatedActionAttributeRoutes(controller)))
				.Select(controller => new RouteSelection()
				{
					ControllerModel = controller,
					ActionModels = controller.Actions.Where(IsPartiallyTranslatedAction).ToList()
				})
				.ToList();
		}
	}
}