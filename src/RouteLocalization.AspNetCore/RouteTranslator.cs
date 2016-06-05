namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class RouteTranslator
	{
		public static string PropertyCultureKey = "culture";

		public RouteTranslator(Configuration configuration, ApplicationModel applicationModel)
		{
			// TODO: Logging

			Configuration = configuration;
			ApplicationModel = applicationModel;

			RouteSelection = new RouteSelection();
			RouteTranslation = new RouteTranslation();
		}

		public ApplicationModel ApplicationModel { get; set; }

		public Configuration Configuration { get; set; }

		public RouteSelection RouteSelection { get; set; }

		public RouteTranslation RouteTranslation { get; set; }

		public RouteTranslator TranslateAction(string template)
		{
			ControllerModel originalControllerModel = GetUntranslatedController();
			ControllerModel translatedControllerModel = TryGetTranslatedController();

			// Check if a translatedControllerModel exist (e.g. would be the case if TranslateAction or TranslateController was already called)
			if (translatedControllerModel == null)
			{
				// TODO: What if controller contains attribute routed controller routes? Should a default route be added?
				translatedControllerModel = CreateTranslatedControllerModel(
					originalControllerModel,
					RouteTranslator.PropertyCultureKey);

				ApplicationModel.Controllers.Add(translatedControllerModel);
			}

			IList<ActionModel> originalActionModels = GetActions(originalControllerModel);

			Debug.Assert(originalActionModels.Count != 0);

			// Can't translate non attribute routed action routes
			if (
				originalActionModels.Any(
					originalActionModel => originalActionModel.Selectors.Any(x => x.AttributeRouteModel == null)))
			{
				throw new InvalidOperationException();
			}

			IList<ActionModel> translatedActionModels = GetActions(translatedControllerModel);

			// Should not happen
			if (originalActionModels.Count != translatedActionModels.Count)
			{
				throw new InvalidOperationException();
			}

			// Multiple actions translated at once should have the same template
			if (
				originalActionModels.Select((model, index) => new { Model = model, Index = index })
					.Any(
						x =>
							x.Model.Selectors[translatedActionModels[x.Index].Selectors.Count].AttributeRouteModel.Template !=
								x.Model.Selectors[translatedActionModels.First().Selectors.Count].AttributeRouteModel.Template))
			{
				throw new InvalidOperationException();
			}

			// Check if not already translated
			if (
				originalActionModels.Select((model, index) => new { Model = model, Index = index })
					.Any(x => x.Model.Selectors.Count == translatedActionModels[x.Index].Selectors.Count))
			{
				throw new InvalidOperationException();
			}

			// Translate
			foreach (
				var originalActionModel in originalActionModels.Select((model, index) => new { Model = model, Index = index }))
			{
				// TODO: Validate Url, AddCultureAsRoutePrefix
				SelectorModel translatedSelectorModel =
					new SelectorModel(
						originalActionModel.Model.Selectors[translatedActionModels[originalActionModel.Index].Selectors.Count])
					{
						AttributeRouteModel = { Template = template }
					};

				translatedActionModels[originalActionModel.Index].Selectors.Add(translatedSelectorModel);
			}

			return this;
		}

		public RouteTranslator TranslateController(string template)
		{
			ControllerModel originalControllerModel = GetUntranslatedController();

			// Can't translate non attribute routed controller routes
			if (originalControllerModel.Selectors.Any(x => x.AttributeRouteModel == null))
			{
				throw new InvalidOperationException();
			}

			ControllerModel translatedControllerModel = TryGetTranslatedController();

			// Check if a translatedControllerModel exist (e.g. would be the case if TranslateController / TranslateAction was already called)
			if (translatedControllerModel == null)
			{
				translatedControllerModel = CreateTranslatedControllerModel(
					originalControllerModel,
					RouteTranslator.PropertyCultureKey);

				ApplicationModel.Controllers.Add(translatedControllerModel);
			}

			// Check if not already translated
			if (originalControllerModel.Selectors.Count == translatedControllerModel.Selectors.Count)
			{
				throw new InvalidOperationException();
			}

			// Translate
			// TODO: Validate url, AddCultureAsRoutePrefix, handle route names
			SelectorModel translatedSelectorModel =
				new SelectorModel(originalControllerModel.Selectors[translatedControllerModel.Selectors.Count])
				{
					AttributeRouteModel = { Template = template }
				};

			translatedControllerModel.Selectors.Add(translatedSelectorModel);

			return this;
		}

		public RouteTranslator UseCulture(string culture)
		{
			RouteTranslation.Culture = culture;

			return this;
		}

		public RouteTranslator WhereAction(string action)
		{
			return WhereAction(action, null);
		}

		public RouteTranslator WhereAction(string action, Type[] actionArguments)
		{
			RouteSelection.Action = action;
			RouteSelection.ActionArguments = actionArguments;

			return this;
		}

		public RouteTranslator<T> WhereController<T>()
		{
			RouteSelection.Controller = Regex.Replace(typeof(T).Name, "Controller$", string.Empty);
			RouteSelection.ControllerNamespace = typeof(T).Namespace;

			return ToGeneric<T>();
		}

		protected virtual ControllerModel CreateTranslatedControllerModel(ControllerModel originalControllerModel,
			string propertyCultureKey)
		{
			ControllerModel translatedControllerModel = new ControllerModel(originalControllerModel);
			translatedControllerModel.RouteValues.Add(propertyCultureKey, RouteTranslation.Culture);

			// Clear existing attribute routed controller routes
			translatedControllerModel.Selectors.Clear();

			foreach (ActionModel action in translatedControllerModel.Actions.ToList())
			{
				// Check for attribute routed action routes
				if (action.Selectors.All(x => x.AttributeRouteModel != null))
				{
					action.RouteValues.Add(propertyCultureKey, RouteTranslation.Culture);
				}

				action.Selectors.Clear();
			}

			return translatedControllerModel;
		}

		protected virtual IList<ActionModel> GetActions(ControllerModel controllerModel)
		{
			// TODO: Named Route on Actions

			return
				controllerModel.Actions.Where(
					action =>
						action.MatchesActionName(RouteSelection.Action) && action.MatchesActionArguments(RouteSelection.ActionArguments))
					.ToList();
		}

		protected virtual ControllerModel GetUntranslatedController()
		{
			return
				ApplicationModel.Controllers.Single(
					x => x.MatchesController(RouteSelection.Controller, RouteSelection.ControllerNamespace) && x.IsUntranslated());
		}

		protected virtual RouteTranslator<T> ToGeneric<T>()
		{
			return new RouteTranslator<T>(Configuration, ApplicationModel)
			{
				RouteSelection = RouteSelection,
				RouteTranslation = RouteTranslation,
			};
		}

		protected virtual ControllerModel TryGetTranslatedController()
		{
			// TODO: Named Route on Controller

			return
				ApplicationModel.Controllers.SingleOrDefault(
					x =>
						x.ControllerName == RouteSelection.Controller && x.ControllerType.Namespace == RouteSelection.ControllerNamespace &&
							x.IsTranslatedFor(RouteTranslation.Culture));
		}
	}
}