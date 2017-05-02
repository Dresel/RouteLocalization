namespace RouteLocalization.AspNetCore
{
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class DefaultLocalizer : ILocalizer
	{
		public DefaultLocalizer() : this("culture")
		{
		}

		public DefaultLocalizer(string cultureKey)
		{
			CultureKey = cultureKey;
		}

		public string CultureKey { get; set; }

		public virtual string ApplyRouteCulturePrefix(string template, string culture)
		{
			return $"[{CultureKey}]/{template}";
		}

		public virtual bool IsLocalizedModel(ControllerModel controllerModel)
		{
			return controllerModel.RouteValues.ContainsKey(CultureKey);
		}

		public virtual bool IsLocalizedModel(ActionModel actionModel)
		{
			return actionModel.RouteValues.ContainsKey(CultureKey);
		}

		public virtual bool IsLocalizedModelFor(ControllerModel controllerModel, string culture)
		{
			return controllerModel.RouteValues.ContainsKey(CultureKey) && controllerModel.RouteValues[CultureKey] == culture;
		}

		public virtual bool IsLocalizedModelFor(ActionModel actionModel, string culture)
		{
			return actionModel.RouteValues.ContainsKey(CultureKey) && actionModel.RouteValues[CultureKey] == culture;
		}

		public virtual bool IsOriginalModel(ControllerModel controllerModel)
		{
			return !controllerModel.RouteValues.ContainsKey(CultureKey);
		}

		public virtual bool IsOriginalModel(ActionModel actionModel)
		{
			return !actionModel.RouteValues.ContainsKey(CultureKey);
		}

		public virtual void MarkModelLocalizedFor(ControllerModel controllerModel, string culture)
		{
			controllerModel.RouteValues.Add(CultureKey, culture);
		}

		public virtual void MarkModelLocalizedFor(ActionModel actionModel, string culture)
		{
			actionModel.RouteValues.Add(CultureKey, culture);
		}
	}
}