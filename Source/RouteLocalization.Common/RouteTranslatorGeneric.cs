#if ASPNETWEBAPI
namespace RouteLocalization.Http
#else
namespace RouteLocalization.Mvc
#endif
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

#if ASPNETWEBAPI
	using RouteLocalization.Http.Routing;
#else
	using RouteLocalization.Mvc.Routing;
#endif

	public class RouteTranslator<T> : RouteTranslator
	{
		public new RouteTranslator<T> AddTranslation(string url)
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

		public new RouteTranslator<T> AddTranslation(string url, string culture)
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

		public new RouteTranslator<T> AddTranslation(string url, string culture, string action)
		{
			return AddTranslation(url, culture, Controller, action, ControllerNamespace, ActionArguments);
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture, string controller, string action,
			string controllerNamespace, ICollection<Type> actionArguments)
		{
			base.AddTranslation(url, culture, controller, action, controllerNamespace, actionArguments);

			return this;
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture, LocalizationCollectionRoute route)
		{
			base.AddTranslation(url, culture, route);

			return this;
		}

		public new RouteTranslator<T> AddTranslationForNamedRoute(string url, string culture, string namedRoute)
		{
			base.AddTranslationForNamedRoute(url, culture, namedRoute);

			return this;
		}

		public RouteTranslator<T> ForAction(Expression<Func<T, object>> expression)
		{
			return ForAction(expression, null);
		}

		public RouteTranslator<T> ForAction(Expression<Func<T, object>> expression, Type[] actionArguments)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			if (methodCall == null)
			{
				throw new ArgumentException("Expression must be a MethodCallExpression", "expression");
			}

			ForAction(methodCall.Method.Name, actionArguments);

			return this;
		}

		public new RouteTranslator<T> ForCulture(string culture)
		{
			base.ForCulture(culture);

			return this;
		}

#if !ASPNETWEBAPI
		public new RouteTranslator<T> SetAreaPrefix(string areaPrefix)
		{
			base.SetAreaPrefix(areaPrefix);

			return this;
		}
#endif

		public new RouteTranslator<T> SetRoutePrefix(string routePrefix)
		{
			base.SetRoutePrefix(routePrefix);

			return this;
		}
	}
}