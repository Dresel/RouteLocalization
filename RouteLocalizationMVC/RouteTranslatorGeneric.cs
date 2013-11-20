namespace RouteLocalizationMVC
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web.Routing;
	using RouteLocalizationMVC.Setup;

	public class RouteTranslator<T> : RouteTranslator
	{
		public new RouteTranslator<T> AddTranslation(string url)
		{
			return AddTranslation(url, Culture, Controller, Action);
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture)
		{
			return AddTranslation(url, culture, Controller, Action);
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture, string action)
		{
			return AddTranslation(url, culture, Controller, action);
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture, string controller, string action)
		{
			ValidateRoutePrefix();
			ValidateRouteArea();

			base.AddTranslation(url, culture, controller, action);

			return this;
		}

		public new RouteTranslator<T> AddTranslation(string url, string culture, Route route)
		{
			ValidateRoutePrefix();
			ValidateRouteArea();

			base.AddTranslation(url, culture, route);

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

			Action = methodCall.Method.Name;
			ActionArguments = actionArguments;

			return this;
		}

		public new RouteTranslator<T> ForCulture(string culture)
		{
			base.ForCulture(culture);

			return this;
		}

		public new RouteTranslator<T> SetAreaPrefix(string areaPrefix)
		{
			base.SetAreaPrefix(areaPrefix);

			return this;
		}

		public new RouteTranslator<T> SetRoutePrefix(string routePrefix)
		{
			base.SetRoutePrefix(routePrefix);

			return this;
		}

		protected void ValidateRouteArea()
		{
			if (Configuration.ValidateRouteArea && !string.IsNullOrEmpty(AreaPrefix))
			{
				if (!typeof(T).GetCustomAttributes(true).Any(x => x.GetType().Name == "RouteAreaAttribute"))
				{
					throw new InvalidOperationException(string.Format("Controller '{0}' does not contain any RouteArea attributes.",
						typeof(T).FullName));
				}
			}
		}

		protected void ValidateRoutePrefix()
		{
			if (Configuration.ValidateRoutePrefix && !string.IsNullOrEmpty(RoutePrefix))
			{
				if (!typeof(T).GetCustomAttributes(true).Any(x => x.GetType().Name == "RoutePrefixAttribute"))
				{
					throw new InvalidOperationException(string.Format("Controller '{0}' does not contain any RoutePrefix attributes.",
						typeof(T).FullName));
				}
			}
		}
	}
}