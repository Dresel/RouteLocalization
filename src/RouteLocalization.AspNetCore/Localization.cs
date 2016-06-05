namespace RouteLocalization.AspNetCore
{
	using System;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class Localization
	{
		public Localization()
		{
			Configuration = new Configuration();
		}

		public Localization Configure(Action<Configuration> configurationAction)
		{
			Configuration = new Configuration();
			configurationAction.Invoke(Configuration);

			return this;
		}

		public RouteTranslator<T> ForController<T>()
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration, ApplicationModel);

			return routeTranslator.WhereController<T>();
		}

		public Configuration Configuration { get; set; }

		public ApplicationModel ApplicationModel { get; set; }

		public RouteTranslator UseCulture(string culture)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration, ApplicationModel);

			return routeTranslator.UseCulture(culture);
		}
	}
}