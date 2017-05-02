namespace RouteLocalization.AspNetCore.Processor
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Selection;

	public class DisableOriginalRouteProcessor : RouteProcessorBase
	{
		public DisableOriginalRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration) : this(
			routeTranslationConfiguration, NullLogger<DisableOriginalRouteProcessor>.Instance)
		{
		}

		public DisableOriginalRouteProcessor(RouteTranslationConfiguration routeTranslationConfiguration,
			ILogger<DisableOriginalRouteProcessor> logger) : base(routeTranslationConfiguration, logger)
		{
		}

		public ICollection<string> Cultures { get; set; }

		public override void Process(ICollection<RouteSelection> routeSelections)
		{
			foreach (RouteSelection routeSelection in routeSelections)
			{
				ControllerModel originalControllerModel = routeSelection.ControllerModel;
				ICollection<ActionModel> originalActionModels = routeSelection.ActionModels;

				originalControllerModel.ThrowIfNotOriginalModel(RouteTranslationConfiguration.Localizer);
				originalActionModels.ThrowIfNotPartOfModel(originalControllerModel);

				// Expected to remove controller route
				if (routeSelection.ActionModels.Count == 0)
				{
					originalControllerModel.ThrowIfNotModelWithAttributeRoutes(RouteTranslationConfiguration.Localizer);

					ICollection<ControllerModel> translatedControllerModels = Cultures
						.Select(culture => originalControllerModel.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture))
						.ToList();

					translatedControllerModels.ThrowIfNotAllPartiallyTranslated(RouteTranslationConfiguration.Localizer);

					// Throw if not controller only attribute route or all action attribute routes completely translated
				}

				bool HasNoActionAttributeRoute(ControllerModel model)
				{
					return originalControllerModel.Actions.All(action => !action.HasAttributeRoutes());
				}

				bool HasCompletelyTranslatedActionAttributeRoutes(ControllerModel model)
				{
					return Cultures.All(culture => model.Actions.All(action => !action.HasAttributeRoutes() || action
						.TryGetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture)
						?.IsCompletelyTranslated(RouteTranslationConfiguration.Localizer) == true));
				}

				// Remove only if controller only attribute route or all action attribute routes completely translated
				if (originalControllerModel.HasAttributeRoutes() && (HasNoActionAttributeRoute(originalControllerModel) ||
					HasCompletelyTranslatedActionAttributeRoutes(originalControllerModel)))
				{
					int minimumNumberOfPartialTranslations = Cultures.Min(culture => originalControllerModel
						.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture)
						.Selectors.Count);

					for (int i = 0; i < minimumNumberOfPartialTranslations; i++)
					{
						if (!originalControllerModel.Selectors[i].ActionConstraints.OfType<NeverAcceptActionContraint>().Any())
						{
							Logger.LogDebug(
								$"Disable selector for controller {originalControllerModel.ControllerName} with template \"{originalControllerModel.Selectors[i].AttributeRouteModel.Template}\".");

							// TODO: Verify if this is the best solution
							originalControllerModel.Selectors[i].ActionConstraints.Add(new NeverAcceptActionContraint());
						}
					}
				}

				if (routeSelection.ActionModels.Count == 0)
				{
					return;
				}

				originalActionModels.ThrowIfNotModelsWithAttributeRoutes(RouteTranslationConfiguration.Localizer);

				foreach (string culture in Cultures)
				{
					IList<ActionModel> translatedActionModels = originalActionModels
						.Select(action => action.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture))
						.ToList();

					translatedActionModels.ThrowIfNotAllPartiallyTranslated(RouteTranslationConfiguration.Localizer);
				}

				foreach (ActionModel originalActionModel in originalActionModels)
				{
					int minimumNumberOfPartialTranslations = Cultures.Min(culture => originalActionModel
						.GetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture)
						.Selectors.Count);

					for (int i = 0; i < minimumNumberOfPartialTranslations; i++)
					{
						if (!originalActionModel.Selectors[i].ActionConstraints.OfType<NeverAcceptActionContraint>().Any())
						{
							Logger.LogDebug(
								$"Disable selector for action {originalActionModel.Controller.ControllerName}:{originalActionModel.ActionName} with template \"{originalActionModel.Selectors[i].AttributeRouteModel.Template}\".");

							// TODO: Verify if this is the best solution
							originalActionModel.Selectors[i].ActionConstraints.Add(new NeverAcceptActionContraint());
						}
					}
				}
			}
		}
	}
}