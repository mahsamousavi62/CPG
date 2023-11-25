using System.Linq;
using FluentAssertions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Events;
using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.Tests.Unit.Helpers;
using Xunit;

namespace CPG.Domain.Tests.Unit.AggregateModels.CPGUserAggregate.CPGUserTests;

public class ReturnBookTests : AggregateTestHelper
{
    private readonly CPGUser _CPGUser;
    private readonly Book _book;
    private readonly DateTimePeriod _dateTimePeriod;

    public ReturnBookTests()
    {
        _CPGUser = GetValidCPGUserAggregate();
        _book = GetValidBookAggregate();
        _dateTimePeriod = GetValidDateTimePeriod();
    }

    private void Act()
        => _CPGUser.ReturnBook(_book.Id);

    [Fact]
    public void when_CPG_user_returns_previously_borrowed_book_should_finish_its_loan()
    {
        // Arrange
        _CPGUser.BorrowBook(_book.Id, _dateTimePeriod);
        
        // Act
        Act();

        // Assert
        _CPGUser.ActiveLoans.Count.Should().Be(0);
        _CPGUser.DomainEvents.Count.Should().BePositive();
        
        var @event = _CPGUser.DomainEvents.Last();
        @event.Should().BeOfType<CPGUserReturnedBookEvent>();
        (@event as CPGUserReturnedBookEvent)?.BookId.Should().Be(_book.Id);
        (@event as CPGUserReturnedBookEvent)?.CPGUserId.Should().Be(_CPGUser.Id);
    }

    [Fact]
    public void when_CPG_user_does_not_have_borrowed_given_book_should_throws_an_exception()
    {
        // Act
        var result = Record.Exception(Act);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CPGUserDoesNotHaveBookBorrowed>();
    }
}