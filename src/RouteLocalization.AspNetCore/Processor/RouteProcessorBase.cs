namespace RouteLocalization.AspNetCore.Processor
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using Microsoft.Extensions.Logging;
	using RouteLocalization.AspNetCore.Selection;

	public abstract class RouteProcessorBase : IRouteProcessor
	{
		protected RouteProcessorBase(RouteTranslationConfiguration routeTranslationConfiguration, ILogger logger)
		{
			RouteTranslationConfiguration = routeTranslationConfiguration;
			Logger = logger;
		}

		public ILogger Logger { get; set; }

		public RouteTranslationConfiguration RouteTranslationConfiguration { get; set; }

		public abstract void Process(ICollection<RouteSelection> routeSelections);

		protected virtual ControllerModel EnsureAndGetLocalizedControllerModel(ControllerModel originalControllerModel,
			string culture)
		{
			ApplicationModel applicationModel = originalControllerModel.Application;
			ControllerModel localizedControllerModel =
				originalControllerModel.TryGetLocalizedModelFor(RouteTranslationConfiguration.Localizer, culture);

			// Check if a localizedControllerModel exist (e.g. would be the case if TranslateController / TranslateAction was already called)
			if (localizedControllerModel == null)
			{
				localizedControllerModel =
					originalControllerModel.CreateLocalizedControllerModel(RouteTranslationConfiguration.Localizer, culture);

				applicationModel.Controllers.Add(localizedControllerModel);

				RouteTranslationConfiguration.OnTranslatedControllerModelCreated(this,
					new OnTranslatedControllerModelCreatedEventArgs()
					{
						OriginalControllerModel = originalControllerModel,
						TranslatedControllerModel = localizedControllerModel
					});
			}

			return localizedControllerModel;
		}

		protected virtual void TranslateActionRoute(ActionModel localizedActionModel, SelectorModel originalSelectorModel,
			string template, string culture)
		{
			Logger.LogDebug(
				$"Add translated selector for action {localizedActionModel.Controller.ControllerName}:{localizedActionModel.ActionName} with \"{template}\" ({culture}).");

			localizedActionModel.ThrowIfNotLocalizedModel(RouteTranslationConfiguration.Localizer);

			if (RouteTranslationConfiguration.ValidateUrl)
			{
				ValidateUrl(originalSelectorModel.AttributeRouteModel.Template, template);
			}

			// Don't add RoutePrefix twice
			if (RouteTranslationConfiguration.AddCultureAsRoutePrefix && !localizedActionModel.Controller
				.GetOriginalModel(RouteTranslationConfiguration.Localizer)
				.HasAttributeRoutes())
			{
				template = RouteTranslationConfiguration.Localizer.ApplyRouteCulturePrefix(template, culture);
			}

			SelectorModel localizedSelectorModel = new SelectorModel(originalSelectorModel)
			{
				AttributeRouteModel =
				{
					Template = template
				}
			};

			localizedActionModel.Selectors.Add(localizedSelectorModel);

			RouteTranslationConfiguration.OnActionRouteTranslated(this, new OnActionRouteTranslatedEventArgs()
			{
				OriginalActionModel = localizedActionModel.GetOriginalModel(RouteTranslationConfiguration.Localizer),
				LocalizedActionModel = localizedActionModel,
				OriginalSelectorModel = originalSelectorModel,
				LocalizedSelectorModel = localizedSelectorModel
			});
		}

		protected virtual void TranslateControllerRoute(ControllerModel localizedControllerModel,
			SelectorModel originalSelectorModel, string template, string culture)
		{
			Logger.LogDebug(
				$"Add translated selector for controller {localizedControllerModel.ControllerName} with \"{template}\" ({culture}).");

			localizedControllerModel.ThrowIfNotLocalizedModel(RouteTranslationConfiguration.Localizer);

			if (RouteTranslationConfiguration.ValidateUrl)
			{
				ValidateUrl(originalSelectorModel.AttributeRouteModel.Template, template);
			}

			if (RouteTranslationConfiguration.AddCultureAsRoutePrefix)
			{
				template = RouteTranslationConfiguration.Localizer.ApplyRouteCulturePrefix(template, culture);
			}

			SelectorModel localizedSelectorModel = new SelectorModel(originalSelectorModel)
			{
				AttributeRouteModel =
				{
					Template = template
				}
			};

			localizedControllerModel.Selectors.Add(localizedSelectorModel);

			RouteTranslationConfiguration.OnControllerRouteTranslated(this, new OnControllerRouteTranslatedEventArgs()
			{
				OriginalControllerModel = localizedControllerModel.GetOriginalModel(RouteTranslationConfiguration.Localizer),
				LocalizedControllerModel = localizedControllerModel,
				OriginalSelectorModel = originalSelectorModel,
				LocalizedSelectorModel = localizedSelectorModel
			});
		}

		protected virtual void ValidateUrl(string originalTemplate, string translatedTemplate)
		{
			MatchCollection originalMatches = Regex.Matches(originalTemplate, "(?<=\\{)[^}]*(?=\\})");
			MatchCollection translationMatches = Regex.Matches(translatedTemplate, "(?<=\\{)[^}]*(?=\\})");

			if (originalMatches.Count != translationMatches.Count)
			{
				throw new InvalidOperationException(string.Format(
					"Template \"{0}\" for translated selector contains different number of {{ }} placeholders than original template \"{1}\"." +
					"Set ValidateUrl to false, if you want to skip validation.", translatedTemplate, originalTemplate));
			}

			for (int i = 0; i < originalMatches.Count; i++)
			{
				if (originalMatches[i].Value != translationMatches[i].Value)
				{
					throw new InvalidOperationException(string.Format(
						"Template \"{0}\" for translated selector contains different {{ }} placeholders than original template \"{1}\"." +
						"Set ValidateURL to false, if you want to skip validation.", translatedTemplate, originalTemplate));
				}
			}

			originalMatches = Regex.Matches(originalTemplate, "(?<=\\[)[^}]*(?=\\])");
			translationMatches = Regex.Matches(translatedTemplate, "(?<=\\[)[^}]*(?=\\])");

			if (originalMatches.Count != translationMatches.Count)
			{
				throw new InvalidOperationException(string.Format(
					"Template \"{0}\" for translated selector contains different number of [ ] placeholders than original template \"{1}\"." +
					"Set ValidateUrl to false, if you want to skip validation.", translatedTemplate, originalTemplate));
			}

			for (int i = 0; i < originalMatches.Count; i++)
			{
				if (originalMatches[i].Value != translationMatches[i].Value)
				{
					throw new InvalidOperationException(string.Format(
						"Template \"{0}\" for translated selector contains different [ ] placeholders than original template \"{1}\"." +
						"Set ValidateURL to false, if you want to skip validation.", translatedTemplate, originalTemplate));
				}
			}
		}
	}
}