using FluentAssertions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.Tests.Unit.Helpers;
using Xunit;

namespace CPG.Domain.Tests.Unit.AggregateModels.BookAggregate.BookTests
{
    public class SetAsTestsNotAvailableTests : AggregateTestHelper
    {
        private readonly Book _book;
        private readonly CPGUser _CPGUser;
        private readonly DateTimePeriod _dateTimePeriod;

        public SetAsTestsNotAvailableTests()
        {
            _CPGUser = GetValidCPGUserAggregate();
            _book = GetValidBookAggregate();
            _dateTimePeriod = GetValidDateTimePeriod();
        }

        private void Act()
            => _book.SetAsNotAvailable(_CPGUser.Id, _dateTimePeriod);

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
