using FluentAssertions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.Tests.Unit.Helpers;
using Xunit;

namespace CPG.Domain.Tests.Unit.AggregateModels.BookAggregate.BookTests
{
    public class SetAsAvailableTests : AggregateTestHelper
    {
        private readonly Book _book;
        private readonly CPGUser _CPGUser;

        public SetAsAvailableTests()
        {
            _CPGUser = GetValidCPGUserAggregate();
            _book = GetValidBookAggregate();
        }
        
        private void Act() 
            => _book.SetAsAvailable(_CPGUser.Id);

        [Fact]
        public void when_book_is_borrowed_book_should_be_returned()
        {
            // Arrange
            _book._inStock = false;

            // Act
            Act();

            // Assert
            _book.InStock.Should().BeTrue();
        }

        [Fact]
        public void when_book_is_not_borrowed_should_throws_an_exception()
        {
            // Arrange
            _book._inStock = true;

            // Act
            var result = Record.Exception(Act);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BookIsInStockException>();
        }
    }
}
