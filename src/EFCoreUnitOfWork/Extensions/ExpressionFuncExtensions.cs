using System;
using System.Linq.Expressions;
using System.Reflection;

namespace EFCoreUnitOfWork.Extensions
{
	public static class ExpressionFuncExtensions
	{
		public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> expression)
		{
			MemberExpression memberExpression;

			switch (expression.Body.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					memberExpression = ((UnaryExpression)expression.Body)?.Operand as MemberExpression;
					break;
				default:
					memberExpression = expression.Body as MemberExpression;
					break;
			}

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
