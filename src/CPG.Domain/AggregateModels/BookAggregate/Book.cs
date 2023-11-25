using System;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.BookAggregate.Events;
using CPG.Domain.AggregateModels.BookAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.BookAggregate;

public class Book : Entity<long>, IAggregateRoot
{
    internal BookInformation _bookInformation;
    internal List<Loan> _loans;
    internal bool _inStock;

    public BookInformation BookInformation => _bookInformation;
    public bool InStock => _inStock;

    internal Book()
    {
        _loans = new List<Loan>();
    }

    private Book(BookInformation bookInformation) : this()
    {
        _bookInformation = bookInformation;
        _inStock = true;
    }

    public static Book Register(string title, string author, string subject, string isbn, long userId)
    {
        var bookInformation = new BookInformation(title, author, subject, isbn);
        var book = new Book(bookInformation);
        
        book.AddDomainEvent(new NewBookRegisteredEvent(book.Id, DateTime.UtcNow));

        return book;
    }
    
    public void SetAsNotAvailable(long CPGUserId, DateTimePeriod borrowPeriod)
    {
        if (!InStock)
            throw new BookIsNotInStockException();
        
        _inStock = false;

        AddDomainEvent(new BookBorrowedEvent(Id, CPGUserId, borrowPeriod));
    }

    public void SetAsAvailable(long CPGUserId)
    {
        if (InStock)
            throw new BookIsInStockException(Id);

        _inStock = true;

        AddDomainEvent(new BookReturnedEvent(Id, DateTime.UtcNow));
    }
}
