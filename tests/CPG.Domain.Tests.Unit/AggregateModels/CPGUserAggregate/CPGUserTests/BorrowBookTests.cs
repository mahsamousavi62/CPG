using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Events;
using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.Tests.Unit.Helpers;
using Xunit;

namespace CPG.Domain.Tests.Unit.AggregateModels.CPGUserAggregate.CPGUserTests
{
    public class BorrowBookTests : AggregateTestHelper
    {
        private readonly CPGUser _CPGUser;
        private readonly Book _book;
        private readonly DateTimePeriod _dateTimePeriod;

        public BorrowBookTests()
        {
            _CPGUser = GetValidCPGUserAggregate();
            _book = GetValidBookAggregate();
            _dateTimePeriod = GetValidDateTimePeriod();
        }

        private void Act()
            => _CPGUser.BorrowBook(_book.Id, _dateTimePeriod);

        [Fact]
        public void when_CPG_user_borrows_available_book_should_has_new_loan_registered()
        {
            // Act
            Act();

            // Assert
            _CPGUser.ActiveLoans.Count.Should().Be(1);
            _CPGUser.ActiveLoans.First().IsActive.Should().BeTrue();
            
            _CPGUser.DomainEvents.Count.Should().Be(1);
            var @event = _CPGUser.DomainEvents.First();
            @event.Should().BeOfType<CPGUserBorrowedBookEvent>();
            (@event as CPGUserBorrowedBookEvent)?.BookId.Should().Be(_book.Id);
            (@event as CPGUserBorrowedBookEvent)?.CPGUserId.Should().Be(_CPGUser.Id);
        }

        [Fact]
        public void when_CPG_user_has_already_exceeded_maximum_value_of_borrowed_books_should_throws_an_exception()
        {
            // Arrange
            _CPGUser._activeLoans.AddRange(new List<Loan>
            {
                GetSampleLoanEntity(),
                GetSampleLoanEntity(),
                GetSampleLoanEntity()
            });

            // Act
            var result = Record.Exception(Act);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CPGUserMaximumBooksBorrowedExceededException>();
        }
    }
}