namespace RouteLocalization.AspNetCore.Sample
{
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;
	using RouteLocalization.AspNetCore.Processor;

	public class CollectRoutesApplicationConvention : IApplicationModelConvention
	{
		public static List<string> Routes { get; set; } = new List<string>();

		public void Apply(ApplicationModel application)
		{
			foreach (ControllerModel controllerModel in application.Controllers)
			{
				string culture;
				if (!controllerModel.RouteValues.TryGetValue("culture", out culture))
				{
					culture = "null";
				}

				ICollection<string> actionTemplates = controllerModel.Actions.SelectMany(action => action.Selectors)
					.Where(selector => selector.AttributeRouteModel != null && !selector.ActionConstraints.OfType<NeverAcceptActionContraint>().Any())
					.Select(selector => selector.AttributeRouteModel.Template)
					.ToList();

				if (controllerModel.Selectors.Any(selector => selector.AttributeRouteModel != null))
				{
					ICollection<string> controllerTemplates = controllerModel.Selectors
						.Where(selector => selector.AttributeRouteModel != null && !selector.ActionConstraints.OfType<NeverAcceptActionContraint>().Any())
						.Select(selector => selector.AttributeRouteModel.Template)
						.ToList();

					if (controllerModel.Actions.Any(action => action.Selectors.All(selector => selector.AttributeRouteModel == null)))
					{
						Routes.AddRange(controllerTemplates.Select(template => $"/{template} [culture:{culture}]"));
					}

					Routes.AddRange(controllerTemplates.SelectMany(actionSelector => actionTemplates,
						(controllerSelector, actionSelector) => $"/{controllerSelector}/{actionSelector} [culture:{culture}]"));
				}
				else
				{
					Routes.AddRange(actionTemplates.Select(template => $"/{template} [culture:{culture}]"));
				}
			}
		}
	}
}