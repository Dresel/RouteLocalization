namespace RouteLocalization.AspNetCore
{
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public interface ILocalizer
	{
		string ApplyRouteCulturePrefix(string template, string culture);

		bool IsLocalizedModel(ControllerModel controllerModel);

		bool IsLocalizedModel(ActionModel actionModel);

		bool IsLocalizedModelFor(ControllerModel controllerModel, string culture);

		bool IsLocalizedModelFor(ActionModel actionModel, string culture);

		bool IsOriginalModel(ControllerModel controllerModel);

		bool IsOriginalModel(ActionModel actionModel);

		void MarkModelLocalizedFor(ControllerModel controllerModel, string culture);

		void MarkModelLocalizedFor(ActionModel actionModel, string culture);
	}
}