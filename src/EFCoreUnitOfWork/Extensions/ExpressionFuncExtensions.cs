using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EFCoreUnitOfWork.Extensions
{
	public static class ExpressionFuncExtensions
	{
		public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;

			if (memberExpression == null)
			{
				throw new ArgumentException("The expression not refer to a property.");
			}

			var propertyInfo = memberExpression.Member as PropertyInfo;

			if (propertyInfo == null)
			{
				throw new ArgumentException("The expression not refer to a property.");
			}

			return propertyInfo;
		}
	}
}
