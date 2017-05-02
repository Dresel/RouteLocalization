namespace RouteLocalization.AspNetCore.Processor
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Selection;

	public class TranslateActionRouteProcessor : RouteProcessorBase
	{
		public TranslateActionRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration) : this(
			routeTranslationConfiguration, NullLogger<TranslateActionRouteProcessor>.Instance)
		{
		}

		public TranslateActionRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration,
			ILogger<TranslateActionRouteProcessor> logger) : base(routeTranslationConfiguration, logger)
		{
		}

		public string Culture { get; set; }

		public string Template { get; set; }

		public override void Process(ICollection<RouteSelection> routeSelections)
		{
			// Expect single ControllerModel with ActionModels selection
			routeSelections.ThrowIfNotSingleControllerWithActionsSelection(RouteTranslationConfiguration.Localizer);

			RouteSelection routeSelection = routeSelections.Single();
			ControllerModel originalControllerModel = routeSelection.ControllerModel;
			ICollection<ActionModel> originalActionModels = routeSelection.ActionModels;

			// Expect original ControllerModel & attribute routed action routes
			originalControllerModel.ThrowIfNotOriginalModel(RouteTranslationConfiguration.Localizer);
			originalActionModels.ThrowIfNotPartOfModel(originalControllerModel);
			originalActionModels.ThrowIfNotModelsWithAttributeRoutes(RouteTranslationConfiguration.Localizer);

			ControllerModel translatedControllerModel = EnsureAndGetLocalizedControllerModel(originalControllerModel, Culture);

			if (originalControllerModel.HasAttributeRoutes() &&
				!translatedControllerModel.IsCompletelyTranslated(RouteTranslationConfiguration.Localizer))
			{
				Logger.LogWarning("Action was translated before Controller. Don't forget to translate on controller level.");
			}

			IList<ActionModel> translatedActionModels = originalActionModels
				.Select(action => action.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, Culture))
				.ToList();

			// Expect at least one untranslated attribute route with the same template for each ActionModel
			translatedActionModels.ThrowIfAnyCompletelyTranslated(RouteTranslationConfiguration.Localizer);
			originalActionModels.ThrowIfAnyUntranslatedTemplateDiffer(RouteTranslationConfiguration.Localizer, Culture);

			// Translate
			foreach (var originalActionModel in originalActionModels.Select((model, index) => new
			{
				Model = model,
				Index = index
			}))
			{
				ActionModel translatedActionModel = translatedActionModels[originalActionModel.Index];

				TranslateActionRoute(translatedActionModel,
					originalActionModel.Model.Selectors[translatedActionModel.Selectors.Count], Template, Culture);
			}
		}
	}
}