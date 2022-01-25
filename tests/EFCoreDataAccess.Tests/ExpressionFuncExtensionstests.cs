using EFCoreDataAccess.Models;
using EFCoreUnitOfWork.Extensions;
using FluentAssertions;
using System;
using System.Linq.Expressions;
using Xunit;

namespace EFCoreDataAccess.Tests
{
	public class ExpressionFuncExtensionstests
	{
        [Fact]
        public void GetPropertyInfo_ShouldThrowArgumentException_GivenMethodExpression()
        {
            // Arrange
            var company = new Company(name: "Disney");

            Expression<Func<Company, object>> expression = (c) => c.ToString();

            // Act
            Action act = () => expression.GetPropertyInfo();

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("The expression not refer to a property.");
        }

        [Fact]
        public void GetPropertyInfo_ShouldReturnThePropertyInfo_GivenPropertyExpression()
        {
            // Arrange
            var company = new Company(name: "Disney");

            var expectedResult = company.GetType().GetProperty("Name");

            Expression<Func<Company, object>> expression = (c) => c.Name;

            // Act
            var result = expression.GetPropertyInfo();

            // Assert
            result.Should().Equals(expectedResult);
        }
    }
}
