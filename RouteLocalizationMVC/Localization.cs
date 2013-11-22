namespace RouteLocalizationMVC
{
	using System;
	using System.Collections.Generic;
	using System.Web.Routing;
	using RouteLocalizationMVC.Setup;

	public class Localization
	{
		public Localization()
		{
			Configuration = new Configuration();
		}

		public Localization(Configuration configuration)
		{
			Configuration = configuration;
		}

		public Configuration Configuration { get; set; }

		public RouteCollection RouteCollection { get; set; }

		public RouteTranslator AddTranslation(string url, string culture, Route route)
		{
			RouteTranslator translator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return translator.AddTranslation(url, culture, route);
		}

		public RouteTranslator AddTranslation(string url, string culture, string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			RouteTranslator translator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return translator.AddTranslation(url, culture, controller, action, controllerNamespace, actionArguments);
		}

		public RouteTranslator AddTranslationForNamedRoute(string url, string culture, string namedRoute)
		{
			RouteTranslator translator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return translator.AddTranslationForNamedRoute(url, culture, namedRoute);
		}

		public RouteTranslator ForController(string controller, string controllerNamespace)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return routeTranslator.ForController(controller, controllerNamespace);
		}

		public RouteTranslator<T> ForController<T>()
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return routeTranslator.ForController<T>();
		}

		public RouteTranslator ForCulture(string culture)
		{
			RouteTranslator translator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return translator.ForCulture(culture);
		}

		public RouteTranslator SetAreaPrefix(string areaPrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return routeTranslator.SetAreaPrefix(areaPrefix);
		}

		public RouteTranslator SetRoutePrefix(string routePrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration) { RouteCollection = RouteCollection };

			return routeTranslator.SetRoutePrefix(routePrefix);
		}

		public void Translate(Action<Localization> localizationAction)
		{
			localizationAction.Invoke(this);
		}
	}
}