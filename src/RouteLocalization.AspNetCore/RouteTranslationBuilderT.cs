namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Microsoft.Extensions.Logging;

	public class RouteTranslationBuilder<T> : RouteTranslationBuilder
	{
		public RouteTranslationBuilder(RouteTranslationConfiguration routeTranslationConfiguration,
			RouteTranslationStore routeTranslationStore, ILoggerFactory loggerFactory) : base(routeTranslationConfiguration,
			routeTranslationStore, loggerFactory)
		{
		}

		public new RouteTranslationBuilder<T> AddDefaultTranslation()
		{
			base.AddDefaultTranslation();

			return this;
		}

		public new RouteTranslationBuilder<T> TranslateAction(string template)
		{
			base.TranslateAction(template);

			return this;
		}

		public new RouteTranslationBuilder<T> TranslateController(string template)
		{
			base.TranslateController(template);

			return this;
		}

		public new RouteTranslationBuilder<T> UseCulture(string culture)
		{
			base.UseCulture(culture);

			return this;
		}

		public new RouteTranslationBuilder<T> UseCultures(string[] cultures)
		{
			base.UseCultures(cultures);

			return this;
		}

		public RouteTranslationBuilder<T> WhereAction(Expression<Action<T>> expression)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			if (methodCall == null)
			{
				throw new ArgumentException("Expression must be a MethodCallExpression", nameof(expression));
			}

			WhereAction(methodCall.Method.Name, methodCall.Arguments.Select(x => x.Type).ToArray());

			return this;
		}

		public new RouteTranslationBuilder<T> WhereAction(string action)
		{
			base.WhereAction(action, null);

			return this;
		}

		public new RouteTranslationBuilder<T> WhereAction(string action, Type[] actionArguments)
		{
			base.WhereAction(action, actionArguments);

			return this;
		}
	}
}