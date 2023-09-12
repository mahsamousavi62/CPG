using System;
using FluentAssertions;
using Daryaftyar.Tests.Base;
using Xunit;

namespace Daryaftyar.Domain.Tests.Unit.AggregateModels.DaryaftyarUserAggregate.DaryaftyarUserId
{
    public class DaryaftyarUserIdCreationTests : TestBase
    {
        private int _idValue;

        private Domain.AggregateModels.DaryaftyarUserAggregate.DaryaftyarUserId Act()
            => new(_idValue);
		
        [Fact]
        public void valid_Id_creates_DaryaftyarUserId()
        {
            // Arrange
            _idValue = CreateInt();
			
            // Act
            var result = Act();
			
            // Assert
            result.Value.Should().Be(_idValue);
        }

        [Theory]
        [MemberData(nameof(IntNegativeAndZeroData))]
        public void negative_or_zero_Id_value_throws_exception(int invalidId)
        {
            // Assert
            _idValue = invalidId;
			
            // Act
            var result = Record.Exception(Act);
			
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ArgumentException>();
        }
    }
}