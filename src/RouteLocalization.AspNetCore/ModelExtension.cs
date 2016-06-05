namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public static class ModelExtension
	{
		public static bool IsTranslatedFor(this ControllerModel controllerModel, string culture)
		{
			return controllerModel.RouteValues.ContainsKey(RouteTranslator.PropertyCultureKey) &&
				(string)controllerModel.RouteValues[RouteTranslator.PropertyCultureKey] == culture;
		}

		public static bool IsTranslatedFor(this ActionModel actionModel, string culture)
		{
			return actionModel.RouteValues.ContainsKey(RouteTranslator.PropertyCultureKey) &&
				(string)actionModel.RouteValues[RouteTranslator.PropertyCultureKey] == culture;
		}

		public static bool IsUntranslated(this ControllerModel controllerModel)
		{
			return !controllerModel.RouteValues.ContainsKey(RouteTranslator.PropertyCultureKey);
		}

		public static bool IsUntranslated(this ActionModel actionModel)
		{
			return !actionModel.RouteValues.ContainsKey(RouteTranslator.PropertyCultureKey);
		}

		public static bool MatchesActionName(this ActionModel actionModel, string actionName)
		{
			return actionModel.ActionName == actionName;
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

		public static bool MatchesController(this ControllerModel controllerModel, string controllerName,
			string controllerNamespace)
		{
			return controllerModel.ControllerName == controllerName &&
				controllerModel.ControllerType.Namespace == controllerNamespace;
		}
	}
}