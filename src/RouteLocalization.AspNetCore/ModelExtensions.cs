namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using RouteLocalization.AspNetCore.Selection;

	public static class ModelExtensions
	{
		public static bool AnyUntranslatedActionModelTemplateDiffer(this ICollection<ActionModel> originalActionModels,
			ILocalizer localizer, string culture)
		{
			ControllerModel originalControllerModel = originalActionModels.First().Controller;

			originalControllerModel.ThrowIfNotOriginalModel(localizer);
			originalActionModels.ThrowIfNotPartOfModel(originalControllerModel);
			originalActionModels.ThrowIfNotModelsWithAttributeRoutes(localizer);

			IList<ActionModel> localizedActionModels = originalActionModels
				.Select(action => action.GetLocalizedModelFor(localizer, culture))
				.ToList();

			string firstUntranslatedTemplate = originalActionModels.First()
				.Selectors[localizedActionModels.First().Selectors.Count]
				.AttributeRouteModel.Template;

			return originalActionModels.Select((model, index) => new
				{
					Model = model,
					Index = index
				})
				.Any(x => x.Model.Selectors[localizedActionModels[x.Index].Selectors.Count].AttributeRouteModel.Template !=
					firstUntranslatedTemplate);
		}

		public static bool AreAllPartiallyTranslated(this ICollection<ControllerModel> localizedControllerModels,
			ILocalizer localizer)
		{
			if (localizedControllerModels.Count == 0)
			{
				return true;
			}

			ControllerModel localizedControllerModel = localizedControllerModels.First();

			localizedControllerModel.ThrowIfNotLocalizedModel(localizer);

			return localizedControllerModels.All(controller => controller.Selectors.Count > 0);
		}

		public static bool AreAllPartiallyTranslated(this ICollection<ActionModel> localizedActionModels,
			ILocalizer localizer)
		{
			if (localizedActionModels.Count == 0)
			{
				return true;
			}

			ControllerModel localizedControllerModel = localizedActionModels.First().Controller;

			localizedControllerModel.ThrowIfNotLocalizedModel(localizer);
			localizedActionModels.ThrowIfNotPartOfModel(localizedControllerModel);
			localizedActionModels.Select(action => action.GetOriginalModel(localizer))
				.ToList()
				.ThrowIfNotModelsWithAttributeRoutes(localizer);

			return localizedActionModels.Select((model, index) => new
				{
					Model = model,
					Index = index
				})
				.All(x => x.Model.Selectors.Count > 0);
		}

		public static ControllerModel CreateLocalizedControllerModel(this ControllerModel originalControllerModel,
			ILocalizer localizer, string culture)
		{
			ControllerModel localizedControllerModel = new ControllerModel(originalControllerModel);
			localizer.MarkModelLocalizedFor(localizedControllerModel, culture);

			// Fix for https://github.com/aspnet/Mvc/issues/6159
			localizedControllerModel.Actions.ToList().ForEach(action => action.Controller = localizedControllerModel);

			// Clear existing attribute routed controller routes
			localizedControllerModel.Selectors.Clear();

			foreach (ActionModel action in localizedControllerModel.Actions.ToList())
			{
				// Check for attribute routed action routes
				if (action.Selectors.All(x => x.AttributeRouteModel != null))
				{
					localizer.MarkModelLocalizedFor(action, culture);
				}

				action.Selectors.Clear();
			}

			return localizedControllerModel;
		}

		public static IEnumerable<IndexedModel<ActionModel>> GetIndexedActionModels(
			this ICollection<ActionModel> actionModels)
		{
			return actionModels.Select((model, index) => new IndexedModel<ActionModel>()
			{
				Model = model,
				Index = index
			});
		}

		public static ControllerModel GetOriginalModel(this ControllerModel localizedControllerModel, ILocalizer localizer)
		{
			localizedControllerModel.ThrowIfNotLocalizedModel(localizer);

			ApplicationModel applicationModel = localizedControllerModel.Application;

			return applicationModel.Controllers.Single(
				controller => controller.MatchesController(localizedControllerModel.ControllerName,
					localizedControllerModel.ControllerType.Namespace) && controller.IsOriginalModel(localizer));
		}

		public static ActionModel GetOriginalModel(this ActionModel localizedActionModel, ILocalizer localizer)
		{
			ControllerModel localizedControllerModel = localizedActionModel.Controller;
			ControllerModel originalControllerModel = localizedControllerModel.GetOriginalModel(localizer);

			return originalControllerModel.Actions.Single(action => action.MatchesActionName(localizedActionModel.ActionName) &&
				action.MatchesActionArguments(localizedActionModel.Parameters));
		}

		public static ControllerModel GetLocalizedModelFor(this ControllerModel originalControllerModel,
			ILocalizer localizer, string culture)
		{
			originalControllerModel.ThrowIfNotOriginalModel(localizer);

			ApplicationModel applicationModel = originalControllerModel.Application;

			return applicationModel.Controllers.Single(
				controller => controller.MatchesController(originalControllerModel.ControllerName,
					originalControllerModel.ControllerType.Namespace) && controller.IsLocalizedModelFor(localizer, culture));
		}

		public static ActionModel GetLocalizedModelFor(this ActionModel originalActionModel, ILocalizer localizer,
			string culture)
		{
			ControllerModel originalControllerModel = originalActionModel.Controller;
			ControllerModel localizedControllerModel = originalControllerModel.GetLocalizedModelFor(localizer, culture);

			return localizedControllerModel.Actions.Single(action => action.MatchesActionName(originalActionModel.ActionName) &&
				action.MatchesActionArguments(originalActionModel.Parameters));
		}

		public static IEnumerable<SelectorModel> GetUntranslatedSelectorsFor(this ControllerModel originalControllerModel,
			ILocalizer localizer, string culture)
		{
			ControllerModel localizedControllerModel = originalControllerModel.GetLocalizedModelFor(localizer, culture);

			return originalControllerModel.Selectors.Skip(localizedControllerModel.Selectors.Count);
		}

		public static IEnumerable<SelectorModel> GetUntranslatedSelectorsFor(this ActionModel originalActionModel,
			ILocalizer localizer, string culture)
		{
			ActionModel localizedActionModel = originalActionModel.GetLocalizedModelFor(localizer, culture);

			return originalActionModel.Selectors.Skip(localizedActionModel.Selectors.Count);
		}

		public static bool HasAttributeRoutes(this ControllerModel controllerModel)
		{
			return controllerModel.Selectors.Any() && controllerModel.Selectors.All(x => x.AttributeRouteModel != null);
		}

		public static bool HasAttributeRoutes(this ActionModel actionModel)
		{
			return actionModel.Selectors.Any() && actionModel.Selectors.All(x => x.AttributeRouteModel != null);
		}

		public static bool IsCompletelyTranslated(this ControllerModel controllerModel, ILocalizer localizer)
		{
			controllerModel.ThrowIfNotLocalizedModel(localizer);

			ControllerModel originalControllerModel = controllerModel.GetOriginalModel(localizer);
			originalControllerModel.ThrowIfNotModelWithAttributeRoutes(localizer);

			return originalControllerModel.Selectors.Count == controllerModel.Selectors.Count;
		}

		public static bool IsCompletelyTranslated(this ActionModel actionModel, ILocalizer localizer)
		{
			actionModel.ThrowIfNotLocalizedModel(localizer);

			ActionModel originalActionModel = actionModel.GetOriginalModel(localizer);
			originalActionModel.ThrowIfNotModelWithAttributeRoutes(localizer);

			return originalActionModel.Selectors.Count == actionModel.Selectors.Count;
		}

		public static bool IsOriginalModel(this ControllerModel controllerModel, ILocalizer localizer)
		{
			return !IsLocalizedModel(controllerModel, localizer);
		}

		public static bool IsOriginalModel(this ActionModel actionModel, ILocalizer localizer)
		{
			return !IsLocalizedModel(actionModel, localizer);
		}

		public static bool IsPartiallyTranslated(this ControllerModel controllerModel, ILocalizer localizer)
		{
			controllerModel.ThrowIfNotLocalizedModel(localizer);

			ControllerModel originalControllerModel = controllerModel.GetOriginalModel(localizer);
			originalControllerModel.ThrowIfNotModelWithAttributeRoutes(localizer);

			return controllerModel.Selectors.Count > 0;
		}

		public static bool IsPartiallyTranslated(this ActionModel actionModel, ILocalizer localizer)
		{
			actionModel.ThrowIfNotLocalizedModel(localizer);

			ActionModel originalActionModel = actionModel.GetOriginalModel(localizer);
			originalActionModel.ThrowIfNotModelWithAttributeRoutes(localizer);

			return actionModel.Selectors.Count > 0;
		}

		public static bool IsLocalizedModel(this ControllerModel controllerModel, ILocalizer localizer)
		{
			return localizer.IsLocalizedModel(controllerModel);
		}

		public static bool IsLocalizedModel(this ActionModel actionModel, ILocalizer localizer)
		{
			return localizer.IsLocalizedModel(actionModel);
		}

		public static bool IsLocalizedModelFor(this ControllerModel controllerModel, ILocalizer localizer, string culture)
		{
			return localizer.IsLocalizedModelFor(controllerModel, culture);
		}

		public static bool IsLocalizedModelFor(this ActionModel actionModel, ILocalizer localizer, string culture)
		{
			return localizer.IsLocalizedModelFor(actionModel, culture);
		}

		public static bool MatchesActionArguments(this ActionModel actionModel, ICollection<Type> actionArguments)
		{
			if (actionArguments == null)
			{
				return true;
			}

			ICollection<ParameterModel> parameterModels = actionModel.Parameters;

			if (parameterModels.Count != actionArguments.Count)
			{
				return false;
			}

			for (int i = 0; i < actionArguments.Count; i++)
			{
				if (actionArguments.ElementAt(i) != parameterModels.ElementAt(i).ParameterInfo.ParameterType)
				{
					return false;
				}
			}

			return true;
		}

		public static bool MatchesActionArguments(this ActionModel actionModel, ICollection<ParameterModel> actionArguments)
		{
			if (actionArguments == null)
			{
				return true;
			}

			ICollection<ParameterModel> parameterModels = actionModel.Parameters;

			if (parameterModels.Count != actionArguments.Count)
			{
				return false;
			}

			for (int i = 0; i < actionArguments.Count; i++)
			{
				if (actionArguments.ElementAt(i).ParameterInfo.ParameterType !=
					parameterModels.ElementAt(i).ParameterInfo.ParameterType)
				{
					return false;
				}
			}

			return true;
		}

		public static bool MatchesActionName(this ActionModel actionModel, string actionName)
		{
			return actionModel.ActionName == actionName;
		}

		public static bool MatchesController(this ControllerModel controllerModel, string controllerName,
			string controllerNamespace)
		{
			return controllerModel.ControllerName == controllerName &&
				(controllerModel.ControllerType.Namespace == controllerNamespace || string.IsNullOrEmpty(controllerNamespace));
		}

		public static void ThrowIfAnyCompletelyTranslated(this ICollection<ActionModel> localizedActionModels,
			ILocalizer localizer)
		{
			if (localizedActionModels.Any(action => action.IsCompletelyTranslated(localizer)))
			{
				throw new InvalidOperationException("Any of ActionModels is completely translated.");
			}
		}

		public static void ThrowIfAnyUntranslatedTemplateDiffer(this ICollection<ActionModel> originalActionModels,
			ILocalizer localizer, string culture)
		{
			if (originalActionModels.AnyUntranslatedActionModelTemplateDiffer(localizer, culture))
			{
				throw new InvalidOperationException(
					"When translating multiple actions at once the template must be the same for all action models.");
			}
		}

		public static void ThrowIfCompletelyTranslated(this ControllerModel controllerModel, ILocalizer localizer)
		{
			if (IsCompletelyTranslated(controllerModel, localizer))
			{
				throw new InvalidOperationException("ControllerModel is completely translated.");
			}
		}

		public static void ThrowIfNotAllPartiallyTranslated(this ICollection<ActionModel> localizedActionModels,
			ILocalizer localizer)
		{
			if (!localizedActionModels.AreAllPartiallyTranslated(localizer))
			{
				throw new InvalidOperationException("Not all ActionModels are partially translated.");
			}
		}

		public static void ThrowIfNotAllPartiallyTranslated(this ICollection<ControllerModel> controllerModels,
			ILocalizer localizer)
		{
			if (!controllerModels.AreAllPartiallyTranslated(localizer))
			{
				throw new InvalidOperationException("Not all ControllerModels are partially translated.");
			}
		}

		public static void ThrowIfNotModelsWithAttributeRoutes(this ICollection<ActionModel> actionModels,
			ILocalizer localizer)
		{
			if (actionModels.Any(action => !action.HasAttributeRoutes()))
			{
				throw new ArgumentException("ActionModels with AttributeRoutes was expected.");
			}
		}

		public static void ThrowIfNotModelWithAttributeRoutes(this ControllerModel controllerModel, ILocalizer localizer)
		{
			controllerModel.ThrowIfNotOriginalModel(localizer);

			if (!controllerModel.HasAttributeRoutes())
			{
				throw new ArgumentException("ControllerModel with AttributeRoutes was expected.");
			}
		}

		public static void ThrowIfNotModelWithAttributeRoutes(this ActionModel actionModel, ILocalizer localizer)
		{
			actionModel.ThrowIfNotOriginalModel(localizer);

			if (!actionModel.HasAttributeRoutes())
			{
				throw new ArgumentException("ActionModel with AttributeRoutes was expected.");
			}
		}

		public static void ThrowIfNotOriginalModel(this ControllerModel controllerModel, ILocalizer localizer)
		{
			if (!controllerModel.IsOriginalModel(localizer))
			{
				throw new ArgumentException("Original ControllerModel was expected.");
			}
		}

		public static void ThrowIfNotOriginalModel(this ActionModel actionModel, ILocalizer localizer)
		{
			if (!actionModel.IsOriginalModel(localizer))
			{
				throw new ArgumentException("Original ControllerModel was expected.");
			}
		}

		public static void ThrowIfNotPartiallyTranslated(this ControllerModel controllerModel, ILocalizer localizer)
		{
			if (!controllerModel.IsPartiallyTranslated(localizer))
			{
				throw new InvalidOperationException("ControllerModel is not partially translated.");
			}
		}

		public static void ThrowIfNotPartOfModel(this ICollection<ActionModel> actionModels, ControllerModel controllerModel)
		{
			if (actionModels.Any(actionModel => !controllerModel.Actions.Contains(actionModel)))
			{
				throw new InvalidOperationException("ActionModels not part of ControllerModel.");
			}
		}

		public static void ThrowIfNotSingleControllerWithActionsSelection(this ICollection<RouteSelection> routeSelections,
			ILocalizer localizer)
		{
			if (routeSelections.Count != 1)
			{
				throw new InvalidOperationException(
					"Multiple RouteSelections given. A single RouteSelection with ControllerModel and ActionModels set expected.");
			}

			if (routeSelections.Single().ActionModels == null)
			{
				throw new InvalidOperationException(
					"Property ActionModels is null. A single RouteSelection with ControllerModel and ActionModels set expected.");
			}
		}

		public static void ThrowIfNotSingleControllerWithoutActionsSelection(this ICollection<RouteSelection> routeSelections,
			ILocalizer localizer)
		{
			if (routeSelections.Count != 1)
			{
				throw new InvalidOperationException(
					"Multiple RouteSelections given. A single RouteSelection with only ControllerModel set expected.");
			}

			if (routeSelections.Single().ActionModels != null)
			{
				throw new InvalidOperationException(
					"Property ActionModels is set. A single RouteSelection with only ControllerModel set expected.");
			}
		}

		public static void ThrowIfNotLocalizedModel(this ControllerModel controllerModel, ILocalizer localizer)
		{
			if (!controllerModel.IsLocalizedModel(localizer))
			{
				throw new ArgumentException("Localized ControllerModel was expected.");
			}
		}

		public static void ThrowIfNotLocalizedModel(this ActionModel actionModel, ILocalizer localizer)
		{
			if (!actionModel.IsLocalizedModel(localizer))
			{
				throw new ArgumentException("Localized ActionModel was expected.");
			}
		}

		public static ControllerModel TryGetLocalizedModelFor(this ControllerModel originalControllerModel,
			ILocalizer localizer, string culture)
		{
			originalControllerModel.ThrowIfNotOriginalModel(localizer);

			ApplicationModel applicationModel = originalControllerModel.Application;

			return applicationModel.Controllers.SingleOrDefault(
				controller => controller.MatchesController(originalControllerModel.ControllerName,
					originalControllerModel.ControllerType.Namespace) && controller.IsLocalizedModelFor(localizer, culture));
		}

		public static ActionModel TryGetLocalizedModelFor(this ActionModel originalActionModel, ILocalizer localizer,
			string culture)
		{
			ControllerModel originalControllerModel = originalActionModel.Controller;
			ControllerModel localizedControllerModel = originalControllerModel.TryGetLocalizedModelFor(localizer, culture);

			return localizedControllerModel?.Actions.Single(
				action => action.MatchesActionName(originalActionModel.ActionName) &&
					action.MatchesActionArguments(originalActionModel.Parameters));
		}
	}
}