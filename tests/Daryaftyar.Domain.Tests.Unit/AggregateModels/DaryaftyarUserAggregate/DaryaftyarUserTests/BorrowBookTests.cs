using System.Collections.Generic;
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
    public class BorrowBookTests : AggregateTestHelper
    {
        private readonly DaryaftyarUser _DaryaftyarUser;
        private readonly Book _book;
        private readonly DateTimePeriod _dateTimePeriod;

        public BorrowBookTests()
        {
            _DaryaftyarUser = GetValidDaryaftyarUserAggregate();
            _book = GetValidBookAggregate();
            _dateTimePeriod = GetValidDateTimePeriod();
        }

        private void Act()
            => _DaryaftyarUser.BorrowBook(_book.Id, _dateTimePeriod);

        [Fact]
        public void when_Daryaftyar_user_borrows_available_book_should_has_new_loan_registered()
        {
            // Act
            Act();

            // Assert
            _DaryaftyarUser.ActiveLoans.Count.Should().Be(1);
            _DaryaftyarUser.ActiveLoans.First().IsActive.Should().BeTrue();
            
            _DaryaftyarUser.DomainEvents.Count.Should().Be(1);
            var @event = _DaryaftyarUser.DomainEvents.First();
            @event.Should().BeOfType<DaryaftyarUserBorrowedBookEvent>();
            (@event as DaryaftyarUserBorrowedBookEvent)?.BookId.Should().Be(_book.Id);
            (@event as DaryaftyarUserBorrowedBookEvent)?.DaryaftyarUserId.Should().Be(_DaryaftyarUser.Id);
        }

        [Fact]
        public void when_Daryaftyar_user_has_already_exceeded_maximum_value_of_borrowed_books_should_throws_an_exception()
        {
            // Arrange
            _DaryaftyarUser._activeLoans.AddRange(new List<Loan>
            {
                GetSampleLoanEntity(),
                GetSampleLoanEntity(),
                GetSampleLoanEntity()
            });

            // Act
            var result = Record.Exception(Act);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DaryaftyarUserMaximumBooksBorrowedExceededException>();
        }
    }
}