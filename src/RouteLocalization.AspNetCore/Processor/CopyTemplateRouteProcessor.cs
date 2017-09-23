namespace RouteLocalization.AspNetCore.Processor
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Selection;

	public class CopyTemplateRouteProcessor : RouteProcessorBase
	{
		public CopyTemplateRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration)
			: this(routeTranslationConfiguration, NullLogger<CopyTemplateRouteProcessor>.Instance)
		{
		}

		public CopyTemplateRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration,
			ILogger<CopyTemplateRouteProcessor> logger) : base(routeTranslationConfiguration, logger)
		{
		}

		public string Culture { get; set; }

		public override void Process(ICollection<RouteSelection> routeSelections)
		{
			foreach (RouteSelection routeSelection in routeSelections)
			{
				ControllerModel originalControllerModel = routeSelection.ControllerModel;
				ICollection<ActionModel> originalActionModels = routeSelection.ActionModels;

				originalControllerModel.ThrowIfNotOriginalModel(RouteTranslationConfiguration.Localizer);
				originalActionModels.ThrowIfNotPartOfModel(originalControllerModel);

				ControllerModel translatedControllerModel = EnsureAndGetLocalizedControllerModel(originalControllerModel, Culture);

				// Expected to translate controller route
				if (routeSelection.ActionModels.Count == 0)
				{
					originalControllerModel.ThrowIfNotModelWithAttributeRoutes(RouteTranslationConfiguration.Localizer);
					translatedControllerModel.ThrowIfCompletelyTranslated(RouteTranslationConfiguration.Localizer);
				}

				if (originalControllerModel.HasAttributeRoutes())
				{
					foreach (SelectorModel selectorModel in originalControllerModel.GetUntranslatedSelectorsFor(RouteTranslationConfiguration.Localizer,
						Culture))
					{
						TranslateControllerRoute(translatedControllerModel, selectorModel, selectorModel.AttributeRouteModel.Template,
							Culture);
					}
				}

				if (routeSelection.ActionModels.Count == 0)
				{
					continue;
				}

				originalActionModels.ThrowIfNotModelsWithAttributeRoutes(RouteTranslationConfiguration.Localizer);

				IList<ActionModel> translatedActionModels = originalActionModels
					.Select(action => action.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, Culture))
					.ToList();

				// Expect at least one untranslated attribute route for each ActionModel
				translatedActionModels.ThrowIfAnyCompletelyTranslated(RouteTranslationConfiguration.Localizer);

				foreach (IndexedModel<ActionModel> indexedModel in originalActionModels.GetIndexedActionModels())
				{
					foreach (SelectorModel selectorModel in indexedModel.Model.GetUntranslatedSelectorsFor(RouteTranslationConfiguration.Localizer,
						Culture))
					{
						TranslateActionRoute(translatedActionModels[indexedModel.Index], selectorModel,
							selectorModel.AttributeRouteModel.Template, Culture);
					}
				}
			}
		}
	}
}