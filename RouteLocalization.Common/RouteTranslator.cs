#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

#if ASPNETWEBAPI
	using System.Web.Http.Routing;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Setup;
#else
	using System.Web.Mvc.Routing;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;
#endif

	public class RouteTranslator
	{
		public RouteTranslator()
		{
			Configuration = new Configuration();
		}

		public RouteTranslator(Configuration configuration)
		{
			Configuration = configuration;
		}

		public string Action { get; set; }

		public ICollection<Type> ActionArguments { get; set; }

		public string AreaPrefix { get; set; }

		public Configuration Configuration { get; set; }

		public string Controller { get; set; }

		public string ControllerNamespace { get; set; }

		public string Culture { get; set; }

		public string NamedRoute { get; set; }

		public ICollection<RouteEntry> RouteEntries
		{
			get
			{
				return Configuration.LocalizationCollectionRoutes;
			}
		}

		public string RoutePrefix { get; set; }

		public RouteTranslator AddNeutralTranslation(LocalizationCollectionRoute route)
		{
			route.AddTranslation(route.Url(), string.Empty);

			return this;
		}

		public RouteTranslator AddTranslation(string url)
		{
			if (string.IsNullOrEmpty(NamedRoute))
			{
				return AddTranslation(url, Culture, Controller, Action, ControllerNamespace, ActionArguments);
			}
			else
			{
				return AddTranslationForNamedRoute(url, Culture, NamedRoute);
			}
		}

		public RouteTranslator AddTranslation(string url, string culture)
		{
			if (string.IsNullOrEmpty(NamedRoute))
			{
				return AddTranslation(url, culture, Controller, Action, ControllerNamespace, ActionArguments);
			}
			else
			{
				return AddTranslationForNamedRoute(url, Culture, NamedRoute);
			}
		}

		public RouteTranslator AddTranslation(string url, string culture, string action)
		{
			return AddTranslation(url, culture, Controller, action, ControllerNamespace, ActionArguments);
		}

		public RouteTranslator AddTranslation(string url, string culture, string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			// At least the controller must be specified
			if (string.IsNullOrEmpty(controller))
			{
				throw new ArgumentNullException("controller");
			}

			ICollection<LocalizationCollectionRoute> routes = new List<LocalizationCollectionRoute>();

			// Translate first route found
			if (!Configuration.AddTranslationToSimiliarUrls)
			{
				LocalizationCollectionRoute route = RouteEntries.GetFirstUntranslatedRoute(culture, controller, action,
					controllerNamespace, actionArguments);

				if (route != null)
				{
					routes.Add(route);
				}
			}
			else
			{
				routes = RouteEntries.GetSimiliarUntranslatedRoutes(culture, controller, action, controllerNamespace,
					actionArguments);
			}

			if (routes.Count == 0)
			{
				throw new InvalidOperationException(string.Format("No Route found for given Controller '{0}' and Action '{1}'.",
					controller, action));
			}

			routes.ToList().ForEach(x => AddTranslation(url, culture, x));

			return this;
		}

		public RouteTranslator AddTranslation(string url, string culture, LocalizationCollectionRoute route)
		{
			if (string.IsNullOrEmpty(culture))
			{
				throw new ArgumentNullException("culture");
			}

			if (route == null)
			{
				throw new ArgumentNullException("route");
			}

			if (Configuration.ValidateCulture && !Configuration.AcceptedCultures.Contains(culture))
			{
				throw new InvalidOperationException(string.Format("AcceptedCultures does not contain culture '{0}'.", culture));
			}

			route.AddTranslation(TransformUrl(url, culture, route), culture);

			if (Configuration.AttributeRouteProcessing == AttributeRouteProcessing.AddAsNeutralRouteAndReplaceByFirstTranslation)
			{
				// Remove neutral route
				route.RemoveTranslation(string.Empty);
			}

			return this;
		}

		public RouteTranslator AddTranslationForNamedRoute(string url, string culture, string namedRoute)
		{
			LocalizationCollectionRoute route = RouteEntries.GetNamedRoute(culture, namedRoute);

			if (route == null)
			{
				throw new InvalidOperationException(string.Format("No Route found for name'{0}'.", namedRoute));
			}

			return AddTranslation(url, culture, route);
		}

		public RouteTranslator ForAction(string action)
		{
			return ForAction(action, null);
		}

		public RouteTranslator ForAction(string action, Type[] actionArguments)
		{
			Action = action;
			ActionArguments = actionArguments;

			return this;
		}

		public RouteTranslator ForController(string controller, string controllerNamespace)
		{
			Controller = controller;
			ControllerNamespace = controllerNamespace;

			return this;
		}

		public RouteTranslator<T> ForController<T>()
		{
			Controller = Regex.Replace(typeof(T).Name, "Controller$", string.Empty);
			ControllerNamespace = typeof(T).Namespace;

			return ToGeneric<T>();
		}

		public RouteTranslator ForCulture(string culture)
		{
			Culture = culture;

			return this;
		}

		public RouteTranslator ForNamedRoute(string namedRoute)
		{
			NamedRoute = namedRoute;

			return this;
		}

		public RouteTranslator SetAreaPrefix(string areaPrefix)
		{
			AreaPrefix = areaPrefix;

			return this;
		}

		public RouteTranslator SetRoutePrefix(string routePrefix)
		{
			RoutePrefix = routePrefix;

			return this;
		}

		protected RouteTranslator<T> ToGeneric<T>()
		{
			return new RouteTranslator<T>
			{
				Action = Action,
				ActionArguments = ActionArguments,
				AreaPrefix = AreaPrefix,
				Configuration = Configuration,
				Controller = Controller,
				ControllerNamespace = ControllerNamespace,
				Culture = Culture,
				NamedRoute = NamedRoute,
				RoutePrefix = RoutePrefix
			};
		}

		protected string TransformUrl(string url, string culture, LocalizationCollectionRoute localizationCollectionRoute)
		{
			// Apply Route and Area Prefix
			url = string.IsNullOrEmpty(RoutePrefix) ? url : string.Format("{0}/{1}", RoutePrefix, url);
			url = string.IsNullOrEmpty(AreaPrefix) ? url : string.Format("{0}/{1}", AreaPrefix, url);
			url = !Configuration.AddCultureAsRoutePrefix ? url : string.Format("{0}/{1}", culture, url);

			// Validate and check if translation has identical placeholders
			if (Configuration.ValidateUrl)
			{
				MatchCollection originalMatches = Regex.Matches(localizationCollectionRoute.Url(), "{.*?}");
				MatchCollection translationMatches = Regex.Matches(url, "{.*?}");

				if (originalMatches.Count != translationMatches.Count)
				{
					throw new InvalidOperationException(
						string.Format(
							"Translation Route '{0}' contains different number of {{ }} placeholders than original Route '{1}'." +
								"Set Configuration.ValidateURL to false, if you want to skip validation.", url,
							localizationCollectionRoute.Url()));
				}

				for (int i = 0; i < originalMatches.Count; i++)
				{
					if (originalMatches[i].Value != translationMatches[i].Value)
					{
						throw new InvalidOperationException(
							string.Format(
								"Translation Route '{0}' contains different {{ }} placeholders than original Route '{1}'." +
									"Set Configuration.ValidateURL to false, if you want to skip validation.", url,
								localizationCollectionRoute.Url()));
					}
				}
			}

			return url;
		}
	}
}