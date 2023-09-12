using System.Linq;
using FluentAssertions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Domain.Tests.Unit.Helpers;
using Xunit;

namespace Daryaftyar.Domain.Tests.Unit.AggregateModels.DaryaftyarUserAggregate.DaryaftyarUserTests
{
    public class ReturnBookTests : AggregateTestHelper
    {
        private readonly DaryaftyarUser _DaryaftyarUser;
        private readonly Book _book;
        private readonly DateTimePeriod _dateTimePeriod;

        public ReturnBookTests()
        {
            _DaryaftyarUser = GetValidDaryaftyarUserAggregate();
            _book = GetValidBookAggregate();
            _dateTimePeriod = GetValidDateTimePeriod();
        }

        private void Act()
            => _DaryaftyarUser.ReturnBook(_book.Id);

        [Fact]
        public void when_Daryaftyar_user_returns_previously_borrowed_book_should_finish_its_loan()
        {
            // Arrange
            _DaryaftyarUser.BorrowBook(_book.Id, _dateTimePeriod);
            
            // Act
            Act();

            // Assert
            _DaryaftyarUser.ActiveLoans.Count.Should().Be(0);
            _DaryaftyarUser.DomainEvents.Count.Should().BePositive();
            
            var @event = _DaryaftyarUser.DomainEvents.Last();
            @event.Should().BeOfType<DaryaftyarUserReturnedBookEvent>();
            (@event as DaryaftyarUserReturnedBookEvent)?.BookId.Should().Be(_book.Id);
            (@event as DaryaftyarUserReturnedBookEvent)?.DaryaftyarUserId.Should().Be(_DaryaftyarUser.Id);
        }

        [Fact]
        public void when_Daryaftyar_user_does_not_have_borrowed_given_book_should_throws_an_exception()
        {
            // Act
            var result = Record.Exception(Act);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DaryaftyarUserDoesNotHaveBookBorrowed>();
        }
    }
}