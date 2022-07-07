using EFCoreDataAccess.Models;
using EFCoreUnitOfWork.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Linq.Expressions;
using Xunit;

namespace EFCoreDataAccess.Tests
{
	public class ExpressionFuncExtensionsTests
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
            var employee = new Employee(name: "Disney", code: "123", position: "Software Developer", birthDate: new DateTime(2000, 02, 10));
            var employeeEarnings = new EmployeeEarnings(Models.Enums.PaymentPeriodType.Monthly);

            var expectedNameResult = employee.GetType().GetProperty("Name");
            var expectedBirthDateResult = employee.GetType().GetProperty("BirthDate");
            var expectedCompanyIdResult = employee.GetType().GetProperty("CompanyId");
            var expectedPaymentPeriodResult = employeeEarnings.GetType().GetProperty("PaymentPeriodType");

            Expression<Func<Employee, object>> nameExpression = (e) => e.Name;
            Expression<Func<Employee, object>> birthDateExpression = (e) => e.BirthDate;
            Expression<Func<Employee, object>> companyIdExpression = (e) => e.CompanyId;
            Expression<Func<EmployeeEarnings, object>> paymentPeriodExpression = (e) => e.PaymentPeriodType;

            // Act
            var nameExpressionResult = nameExpression.GetPropertyInfo();
            var birthDateExprssionResult = birthDateExpression.GetPropertyInfo();
            var companyIdExprssionResult = companyIdExpression.GetPropertyInfo();
            var paymentPeriodExprssionResult = paymentPeriodExpression.GetPropertyInfo();

            // Assert
            using (new AssertionScope())
            {
                nameExpressionResult.Should().Equals(expectedNameResult);
                birthDateExprssionResult.Should().Equals(expectedBirthDateResult);
                companyIdExprssionResult.Should().Equals(expectedCompanyIdResult);
                paymentPeriodExprssionResult.Should().Equals(expectedPaymentPeriodResult);
            }
        }
    }
}
