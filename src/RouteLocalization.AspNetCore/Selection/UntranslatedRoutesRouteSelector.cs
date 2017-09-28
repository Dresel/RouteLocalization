namespace RouteLocalization.AspNetCore.Selection
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class UntranslatedRoutesRouteSelector : IRouteSelector
	{
		public string Culture { get; set; }

		public ILocalizer Localizer { get; set; }

		public static bool HasNotCompletelyTranslatedActionAttributeRoute(IEnumerable<ActionModel> actions, string culture,
			ILocalizer localizer)
		{
			return actions.Any(action => IsNotCompletelyTranslatedAction(action, culture, localizer));
		}

		public static bool HasNotCompletelyTranslatedControllerAttributeRoute(ControllerModel controller, string culture,
			ILocalizer localizer)
		{
			return controller.HasAttributeRoutes() && controller.TryGetLocalizedModelFor(localizer, culture)
				?.IsCompletelyTranslated(localizer) != true;
		}

		public static bool IsNotCompletelyTranslatedAction(ActionModel action, string culture, ILocalizer localizer)
		{
			return action.HasAttributeRoutes() && action.TryGetLocalizedModelFor(localizer, culture)
				?.IsCompletelyTranslated(localizer) != true;
		}

		public ICollection<RouteSelection> Select(ApplicationModel applicationModel)
		{
			return applicationModel.Controllers.Where(controller => controller.IsOriginalModel(Localizer) &&
				(HasNotCompletelyTranslatedControllerAttributeRoute(controller) ||
					HasNotCompletelyTranslatedActionAttributeRoute(controller.Actions))).Select(controller => new RouteSelection()
			{
				ControllerModel = controller,
				ActionModels = controller.Actions.Where(IsNotCompletelyTranslatedAction).ToList()
			}).ToList();
		}

		protected bool HasNotCompletelyTranslatedActionAttributeRoute(IEnumerable<ActionModel> actions) =>
			HasNotCompletelyTranslatedActionAttributeRoute(actions, Culture, Localizer);

		protected bool HasNotCompletelyTranslatedControllerAttributeRoute(ControllerModel controller) =>
			HasNotCompletelyTranslatedControllerAttributeRoute(controller, Culture, Localizer);

		protected bool IsNotCompletelyTranslatedAction(ActionModel action) => IsNotCompletelyTranslatedAction(action, Culture,
			Localizer);
	}
}