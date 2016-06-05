namespace RouteLocalization.AspNetCore
{
	using System;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class RouteLocalizationConvention : IApplicationModelConvention
	{
		public RouteLocalizationConvention(Action<Localization> setupAction)
		{
			Setup = setupAction;
		}

		public Action<Localization> Setup { get; set; }

		public void Apply(ApplicationModel application)
		{
			Localization localization = new Localization { ApplicationModel = application };

			Setup.Invoke(localization);
		}
	}
}