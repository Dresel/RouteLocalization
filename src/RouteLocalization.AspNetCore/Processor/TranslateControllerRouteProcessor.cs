namespace RouteLocalization.AspNetCore.Processor
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Selection;

	public class TranslateControllerRouteProcessor : RouteProcessorBase
	{
		public TranslateControllerRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration) : this(
			routeTranslationConfiguration, NullLogger<TranslateControllerRouteProcessor>.Instance)
		{
		}

		public TranslateControllerRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration,
			ILogger<TranslateControllerRouteProcessor> logger) : base(routeTranslationConfiguration, logger)
		{
		}

		public string Culture { get; set; }

		public string Template { get; set; }

		public override void Process(ICollection<RouteSelection> routeSelections)
		{
			// Expect single ControllerModel selection
			routeSelections.ThrowIfNotSingleControllerWithoutActionsSelection(RouteTranslationConfiguration.Localizer);

			RouteSelection routeSelection = routeSelections.Single();
			ControllerModel originalControllerModel = routeSelection.ControllerModel;

			// Expect original ControllerModel & attribute routed controller routes
			originalControllerModel.ThrowIfNotOriginalModel(RouteTranslationConfiguration.Localizer);
			originalControllerModel.ThrowIfNotModelWithAttributeRoutes(RouteTranslationConfiguration.Localizer);

			ControllerModel translatedControllerModel = EnsureAndGetLocalizedControllerModel(originalControllerModel, Culture);

			// Expect at least one untranslated attribute route for ControllerModel
			translatedControllerModel.ThrowIfCompletelyTranslated(RouteTranslationConfiguration.Localizer);

			TranslateControllerRoute(translatedControllerModel,
				originalControllerModel.Selectors[translatedControllerModel.Selectors.Count], Template, Culture);
		}
	}
}