namespace RouteLocalization.AspNetCore
{
	using System;
	using RouteLocalization.AspNetCore.Processor;

	public class RouteTranslationConfiguration
	{
		public RouteTranslationConfiguration()
		{
		}

		public RouteTranslationConfiguration(ILocalizer localizer)
		{
			Localizer = localizer;
		}

		public bool AddCultureAsRoutePrefix { get; set; } = true;

		public ILocalizer Localizer { get; set; } = new DefaultLocalizer();

		public EventHandler<OnActionRouteTranslatedEventArgs> OnActionRouteTranslated { get; set; } = (sender, args) => { };

		public EventHandler<OnControllerRouteTranslatedEventArgs> OnControllerRouteTranslated { get; set; } =
			(sender, args) => { };

		public EventHandler<OnTranslatedControllerModelCreatedEventArgs> OnTranslatedControllerModelCreated { get; set; } =
			(sender, args) => { };

		public bool ValidateUrl { get; set; } = true;

		public RouteTranslationConfiguration Clone()
		{
			return new RouteTranslationConfiguration
			{
				AddCultureAsRoutePrefix = AddCultureAsRoutePrefix,
				Localizer = Localizer,
				OnActionRouteTranslated = OnActionRouteTranslated,
				OnControllerRouteTranslated = OnControllerRouteTranslated,
				OnTranslatedControllerModelCreated = OnTranslatedControllerModelCreated,
				ValidateUrl = ValidateUrl
			};
		}
	}
}