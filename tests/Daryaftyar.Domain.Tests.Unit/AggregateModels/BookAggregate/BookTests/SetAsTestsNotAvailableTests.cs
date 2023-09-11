using FluentAssertions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.BookAggregate.Exceptions;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Domain.Tests.Unit.Helpers;
using Xunit;

namespace Daryaftyar.Domain.Tests.Unit.AggregateModels.BookAggregate.BookTests
{
    public class SetAsTestsNotAvailableTests : AggregateTestHelper
    {
        private readonly Book _book;
        private readonly DaryaftyarUser _DaryaftyarUser;
        private readonly DateTimePeriod _dateTimePeriod;

        public SetAsTestsNotAvailableTests()
        {
            _DaryaftyarUser = GetValidDaryaftyarUserAggregate();
            _book = GetValidBookAggregate();
            _dateTimePeriod = GetValidDateTimePeriod();
        }

        private void Act()
            => _book.SetAsNotAvailable(_DaryaftyarUser.Id, _dateTimePeriod);

        [Fact]
        public void when_book_is_not_borrowed_should_be_out_of_stock()
        {
            // Arrange
            _book._inStock = true;

            // Act
            Act();

            // Assert
            _book.InStock.Should().BeFalse();
        }

        [Fact]
        public void when_book_is_already_borrowed_should_throws_an_exception()
        {
            // Arrange
            _book._inStock = false;

            // Act
            var result = Record.Exception(Act);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BookIsNotInStockException>();
        }
    }
}
