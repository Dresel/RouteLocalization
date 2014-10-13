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
	using System.Web.Http;
	using System.Web.Http.Routing;
	using RouteLocalization.Http.Extensions;
	using RouteLocalization.Http.Routing;
	using RouteLocalization.Http.Setup;
	using TActionDescriptor = System.Web.Http.Controllers.HttpActionDescriptor;
#else
	using System.Web.Mvc;
	using System.Web.Mvc.Routing;
	using RouteLocalization.Mvc.Extensions;
	using RouteLocalization.Mvc.Routing;
	using RouteLocalization.Mvc.Setup;
	using TActionDescriptor = System.Web.Mvc.ActionDescriptor;
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

#if !ASPNETWEBAPI
		public string AreaPrefix { get; set; }
#endif

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

		public RouteTranslator AddNeutralTranslation()
		{
			if (string.IsNullOrEmpty(NamedRoute))
			{
				return AddNeutralTranslation(Controller, Action, ControllerNamespace, ActionArguments);
			}
			else
			{
				return AddNeutralTranslationForNamedRoute(NamedRoute);
			}
		}

		public RouteTranslator AddNeutralTranslation(LocalizationCollectionRoute route)
		{
			route.AddTranslation(route.Url(), string.Empty);

			return this;
		}

		public RouteTranslator AddNeutralTranslation(string controller, string action, string controllerNamespace,
			ICollection<Type> actionArguments)
		{
			foreach (LocalizationCollectionRoute route in
				GetRoutes(controller, action, controllerNamespace, actionArguments))
			{
				AddNeutralTranslation(route);
			}

			return this;
		}

		public RouteTranslator AddNeutralTranslationForNamedRoute(string namedRoute)
		{
			return AddNeutralTranslation(GetNamedRoute(namedRoute));
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
			foreach (LocalizationCollectionRoute route in
				GetRoutes(controller, action, controllerNamespace, actionArguments))
			{
				AddTranslation(url, culture, route);
			}

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

			if (Configuration.ValidateCulture &&
				Configuration.AcceptedCultures.All(
					acceptedCulture => !string.Equals(acceptedCulture, culture, StringComparison.CurrentCultureIgnoreCase)))
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
			return AddTranslation(url, culture, GetNamedRoute(namedRoute));
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

		public LocalizationCollectionRoute GetNamedRoute(string namedRoute)
		{
			LocalizationCollectionRoute route = RouteEntries.GetNamedRoute(namedRoute);

			if (route == null)
			{
				throw new InvalidOperationException(string.Format("No Route found for name'{0}'.", namedRoute));
			}

			return route;
		}

		public IEnumerable<LocalizationCollectionRoute> GetRoutes(string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			// At least the controller must be specified
			if (string.IsNullOrEmpty(controller))
			{
				throw new ArgumentNullException("controller");
			}

			ICollection<LocalizationCollectionRoute> localizationCollectionRoutes = RouteEntries.GetRoutes(controller, action,
				controllerNamespace, actionArguments);

			if (localizationCollectionRoutes.Count == 0)
			{
				throw new InvalidOperationException(string.Format("No Route found for given Controller '{0}' and Action '{1}'.",
					controller, action));
			}

			if (localizationCollectionRoutes.Count == 1)
			{
				return localizationCollectionRoutes;
			}

			if (!Configuration.AddTranslationToSimiliarUrls)
			{
				throw new InvalidOperationException(
					string.Format(
						"Multiple Routes found for given Controller '{0}' and Action '{1}'." +
							" Narrow down your selection or use AddTranslationToSimiliarUrls if you want to translate similiar Routes at once.",
						controller, action));
			}

			LocalizationCollectionRoute localizationCollectionRoute = localizationCollectionRoutes.First();

			if (localizationCollectionRoutes.Any(x => x.Url() != localizationCollectionRoute.Url()))
			{
				throw new InvalidOperationException(
					string.Format(
						"Multiple Routes with different Url found for given Controller '{0}' and Action '{1}'." +
							" Narrow down your selection.", controller, action));
			}

			return localizationCollectionRoutes;
		}

#if !ASPNETWEBAPI
		public RouteTranslator SetAreaPrefix(string areaPrefix)
		{
			AreaPrefix = areaPrefix;

			return this;
		}
#endif

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

#if !ASPNETWEBAPI
				AreaPrefix = AreaPrefix,
#endif

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
#if !ASPNETWEBAPI
			ValidateRouteArea(localizationCollectionRoute);
#endif

			ValidateRoutePrefix(localizationCollectionRoute);

			// Apply Route and Area Prefix
			url = string.IsNullOrEmpty(RoutePrefix) ? url : string.Format("{0}/{1}", RoutePrefix, url);

#if !ASPNETWEBAPI
			url = string.IsNullOrEmpty(AreaPrefix) ? url : string.Format("{0}/{1}", AreaPrefix, url);
#endif

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

#if !ASPNETWEBAPI
		protected void ValidateRouteArea(LocalizationCollectionRoute localizationCollectionRoute)
		{
			TActionDescriptor actionDescriptor =
				((TActionDescriptor[])localizationCollectionRoute.DataTokens[RouteDataTokenKeys.Actions]).First();
			Type controllerType = actionDescriptor.ControllerDescriptor.ControllerType;

			RouteAreaAttribute routeAreaAttribute =
				controllerType.GetCustomAttributes(true).OfType<RouteAreaAttribute>().SingleOrDefault();

			if (routeAreaAttribute == null)
			{
				if (!string.IsNullOrEmpty(AreaPrefix) && Configuration.ValidateRouteArea)
				{
					throw new InvalidOperationException(
						string.Format(
							"AreaPrefix is set but Controller '{0}' does not contain any RouteArea attributes." +
								"Set Configuration.ValidateRouteArea to false, if you want to skip validation.", controllerType.FullName));
				}
			}
			else if (string.IsNullOrEmpty(AreaPrefix))
			{
				// Use untranslated area name / prefix from attribute
				AreaPrefix = routeAreaAttribute.AreaPrefix ?? routeAreaAttribute.AreaName;
			}
		}
#endif

		protected void ValidateRoutePrefix(LocalizationCollectionRoute localizationCollectionRoute)
		{
			TActionDescriptor actionDescriptor =
				((TActionDescriptor[])localizationCollectionRoute.DataTokens[RouteDataTokenKeys.Actions]).First();

			Type controllerType = actionDescriptor.ControllerDescriptor.ControllerType;

			RoutePrefixAttribute routePrefixAttribute =
				controllerType.GetCustomAttributes(true).OfType<RoutePrefixAttribute>().SingleOrDefault();

			if (routePrefixAttribute == null)
			{
				if (!string.IsNullOrEmpty(RoutePrefix) && Configuration.ValidateRoutePrefix)
				{
					throw new InvalidOperationException(
						string.Format(
							"RoutePrefix is set but Controller '{0}' does not contain any RoutePrefix attributes." +
								"Set Configuration.ValidateRoutePrefix to false, if you want to skip validation.", controllerType.FullName));
				}
			}
			else if (string.IsNullOrEmpty(RoutePrefix))
			{
				// Use untranslated prefix from attribute
				RoutePrefix = routePrefixAttribute.Prefix;
			}
		}
	}
}