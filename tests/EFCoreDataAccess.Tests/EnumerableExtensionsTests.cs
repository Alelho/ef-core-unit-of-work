using EFCoreDataAccess.Extensions;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace EFCoreDataAccess.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IsNullOrEmpty_ShouldReturnTrue_GivenNullableList()
        {
            // Arrange
            List<int> integerList = null;

            // Act
            var result = integerList.IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_ShouldReturnTrue_GivenAnEmptyList()
        {
            // Arrange
            var integerList = new List<int>();

            // Act
            var result = integerList.IsNullOrEmpty();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_ShouldReturnFalse_GivenNonEmptyList()
        {
            // Arrange
            var integerList = new List<int>() { 1 };

            // Act
            var result = integerList.IsNullOrEmpty();

            // Assert
            result.Should().BeFalse();
        }
    }
}
