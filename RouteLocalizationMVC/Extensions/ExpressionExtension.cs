namespace RouteLocalizationMVC.Extensions
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class ExpressionExtension
	{
		public static MethodInfo GetMethodInfo<TType, TMethod>(Expression<Func<TType, TMethod>> expression)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			if (methodCall != null)
			{
				return methodCall.Method;
			}

			throw new ArgumentException("Expression must be a MethodCallExpression", "expression");
		}
	}
}