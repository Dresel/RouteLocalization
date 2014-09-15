#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

#if ASPNETWEBAPI
	using System.Net.Http;
	using System.Web.Http.Routing;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Setup;
#else
	using System.Web;
	using System.Web.Mvc.Routing;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;
#endif

	public class Localization
	{
		private static LocalizationDirectRouteProvider localizationDirectRouteProvider;

		public Localization()
		{
			Configuration = new Configuration();
		}

		public Localization(Configuration configuration)
		{
			Configuration = configuration;
		}

		public static LocalizationDirectRouteProvider LocalizationDirectRouteProvider
		{
			get
			{
				if (localizationDirectRouteProvider == null)
				{
					localizationDirectRouteProvider = new LocalizationDirectRouteProvider(new DefaultDirectRouteProvider());
				}

				return localizationDirectRouteProvider;
			}

			set
			{
				localizationDirectRouteProvider = value;
			}
		}

		public Configuration Configuration { get; set; }

#if ASPNETWEBAPI
		public static Func<HttpRequestMessage, CultureInfo> DetectCultureFromBrowserUserLanguages(
			HashSet<string> acceptedCultures, string defaultCulture)
#else
		public static Func<HttpContext, CultureInfo> DetectCultureFromBrowserUserLanguages(HashSet<string> acceptedCultures,
			string defaultCulture)
#endif
		{
			return (httpObject) =>
			{
				// Set default culture as fallback
				string cultureName = defaultCulture;

#if ASPNETWEBAPI
				if (httpObject.Headers.AcceptLanguage != null)
				{
					// Get language from HTTP Header
					foreach (
						string userLanguage in httpObject.Headers.AcceptLanguage.ToList().Select(x => x.Value.Split(';').First()))
#else
				if (httpObject.Request.UserLanguages != null)
				{
					// Get language from HTTP Header
					foreach (string userLanguage in httpObject.Request.UserLanguages.Select(x => x.Split(';').First()))
#endif
					{
						try
						{
							CultureInfo userCultureInfo = new CultureInfo(userLanguage);

							// We don't can / want to support all languages
							if (acceptedCultures.All(acceptedCulture => !string.Equals(acceptedCulture, userCultureInfo.Name,
								StringComparison.CurrentCultureIgnoreCase)))
							{
								continue;
							}

							// Culture found that is supported
							cultureName = userCultureInfo.Name;
							break;
						}
						catch
						{
							// Ignore invalid cultures
							continue;
						}
					}
				}

				// Return accepted culture
				return new CultureInfo(cultureName);
			};
		}

		public RouteTranslator AddNeutralTranslation(LocalizationCollectionRoute route)
		{
			RouteTranslator translator = new RouteTranslator(Configuration);

			return translator.AddNeutralTranslation(route);
		}

		public RouteTranslator AddTranslation(string url, string culture, LocalizationCollectionRoute route)
		{
			RouteTranslator translator = new RouteTranslator(Configuration);

			return translator.AddTranslation(url, culture, route);
		}

		public RouteTranslator AddTranslation(string url, string culture, string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			RouteTranslator translator = new RouteTranslator(Configuration);

			return translator.AddTranslation(url, culture, controller, action, controllerNamespace, actionArguments);
		}

		public RouteTranslator AddTranslationForNamedRoute(string url, string culture, string namedRoute)
		{
			RouteTranslator translator = new RouteTranslator(Configuration);

			return translator.AddTranslationForNamedRoute(url, culture, namedRoute);
		}

		public RouteTranslator ForController(string controller, string controllerNamespace)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration);

			return routeTranslator.ForController(controller, controllerNamespace);
		}

		public RouteTranslator<T> ForController<T>()
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration);

			return routeTranslator.ForController<T>();
		}

		public RouteTranslator ForCulture(string culture)
		{
			RouteTranslator translator = new RouteTranslator(Configuration);

			return translator.ForCulture(culture);
		}

		public RouteTranslator SetAreaPrefix(string areaPrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration);

			return routeTranslator.SetAreaPrefix(areaPrefix);
		}

		public RouteTranslator SetRoutePrefix(string routePrefix)
		{
			RouteTranslator routeTranslator = new RouteTranslator(Configuration);

			return routeTranslator.SetRoutePrefix(routePrefix);
		}

		public void Translate(Action<Localization> localizationAction)
		{
			localizationAction.Invoke(this);
		}

		public Localization TranslateInitialAttributeRoutes()
		{
			switch (Configuration.AttributeRouteProcessing)
			{
				case AttributeRouteProcessing.None:
					break;

				case AttributeRouteProcessing.AddAsNeutralRoute:
				case AttributeRouteProcessing.AddAsNeutralRouteAndReplaceByFirstTranslation:
					// Add existing attribute route as neutral route
					Configuration.LocalizationCollectionRoutes.ForEach(
						route => AddNeutralTranslation((LocalizationCollectionRoute)route.Route));
					break;

				case AttributeRouteProcessing.AddAsDefaultCultureRoute:
					// Add existing attribute route as default culture route
					Configuration.LocalizationCollectionRoutes.ForEach(
						route => AddTranslation(route.Route.Url(), Configuration.DefaultCulture, (LocalizationCollectionRoute)route.Route));
					break;

				case AttributeRouteProcessing.AddAsNeutralAndDefaultCultureRoute:
					// Add existing attribute route as neutral and default culture route
					Configuration.LocalizationCollectionRoutes.ForEach(
						route => AddNeutralTranslation((LocalizationCollectionRoute)route.Route));
					Configuration.LocalizationCollectionRoutes.ForEach(
						route => AddTranslation(route.Route.Url(), Configuration.DefaultCulture, (LocalizationCollectionRoute)route.Route));
					break;
			}

			return this;
		}
	}
}