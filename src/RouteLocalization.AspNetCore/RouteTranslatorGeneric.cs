namespace RouteLocalization.AspNetCore
{
	using System;
	using System.Linq.Expressions;
	using Microsoft.AspNetCore.Mvc.ApplicationModels;

	public class RouteTranslator<T> : RouteTranslator
	{
		public RouteTranslator(Configuration configuration, ApplicationModel applicationModel)
			: base(configuration, applicationModel)
		{
		}

		public new RouteTranslator<T> TranslateAction(string template)
		{
			base.TranslateAction(template);

			return this;
		}

		public new RouteTranslator<T> TranslateController(string template)
		{
			base.TranslateController(template);

			return this;
		}

		public new RouteTranslator<T> UseCulture(string culture)
		{
			base.UseCulture(culture);

			return this;
		}

		public RouteTranslator<T> WhereAction(Expression<Action<T>> expression)
		{
			return WhereAction(expression, null);
		}

		public RouteTranslator<T> WhereAction(Expression<Action<T>> expression, Type[] actionArguments)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			if (methodCall == null)
			{
				throw new ArgumentException("Expression must be a MethodCallExpression", nameof(expression));
			}

			WhereAction(methodCall.Method.Name, actionArguments);

			return this;
		}

		public new RouteTranslator<T> WhereAction(string action)
		{
			base.WhereAction(action, null);

			return this;
		}

		public new RouteTranslator<T> WhereAction(string action, Type[] actionArguments)
		{
			base.WhereAction(action, actionArguments);

			return this;
		}

		public new RouteTranslator<T> WhereController<T>()
		{
			return base.WhereController<T>();
		}
	}
}