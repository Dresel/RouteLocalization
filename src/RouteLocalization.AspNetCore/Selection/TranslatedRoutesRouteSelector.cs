namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class TranslatedRoutesRouteSelector : IRouteSelector
	{
		public ICollection<string> Cultures { get; set; }

		public ILocalizer Localizer { get; set; }

		public static bool HasPartiallyTranslatedActionAttributeRoutes(IEnumerable<ActionModel> actions,
			ICollection<string> cultures, ILocalizer localizer)
		{
			return actions.Any(action => IsPartiallyTranslatedAction(action, cultures, localizer));
		}

		public static bool HasPartiallyTranslatedControllerOnlyAttributeRoute(ControllerModel controller, IEnumerable<ActionModel> actions,
			ICollection<string> cultures, ILocalizer localizer)
		{
			return controller.HasAttributeRoutes() && !actions.Any(action => action.HasAttributeRoutes()) &&
				cultures.All(culture => controller.TryGetLocalizedModelFor(localizer, culture)?.IsPartiallyTranslated(localizer) ==
					true);
		}

		public static bool IsPartiallyTranslatedAction(ActionModel action, ICollection<string> cultures, ILocalizer localizer)
		{
			return action.HasAttributeRoutes() &&
				cultures.All(
					culture => action.TryGetLocalizedModelFor(localizer, culture)?.IsPartiallyTranslated(localizer) == true);
		}

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			return applicationModel.Controllers.Where(controller => controller.IsOriginalModel(Localizer) &&
				(HasPartiallyTranslatedControllerOnlyAttributeRoute(controller) ||
					HasPartiallyTranslatedActionAttributeRoutes(controller.Actions))).Select(controller => new RouteSelection()
			{
				ControllerModel = controller,
				ActionModels = controller.Actions.Where(IsPartiallyTranslatedAction).ToList()
			}).ToList();
		}

		protected bool HasPartiallyTranslatedActionAttributeRoutes(IEnumerable<ActionModel> actions) =>
			HasPartiallyTranslatedActionAttributeRoutes(actions, Cultures, Localizer);

		protected bool HasPartiallyTranslatedControllerOnlyAttributeRoute(ControllerModel controller) =>
			HasPartiallyTranslatedControllerOnlyAttributeRoute(controller, controller.Actions, Cultures, Localizer);

		protected bool IsPartiallyTranslatedAction(ActionModel action) => IsPartiallyTranslatedAction(action, Cultures,
			Localizer);
	}
}