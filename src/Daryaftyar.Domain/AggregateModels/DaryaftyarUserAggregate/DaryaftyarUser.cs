using System.Collections.Generic;
using System.Linq;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions;
using Daryaftyar.Domain.SeedWork;
using Daryaftyar.Domain.SharedKernel;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate
{
    public class DaryaftyarUser : Entity<long>, IAggregateRoot
    {
        private UserCredential _credentials;
        private string _firstName;
        private string _lastName;
        private Email _email;
        private bool _isActive;
        internal List<Loan> _activeLoans;

        public UserCredential Credentials => _credentials;
        public string FirstName => _firstName;
        public string LastName => _lastName;
        public Email Email => _email;
        public bool IsActive => _isActive;
        public IReadOnlyCollection<Loan> ActiveLoans => _activeLoans.Where(x => x.IsActive).ToList();

        internal DaryaftyarUser()
        {
            _activeLoans = new List<Loan>();
        }

        private DaryaftyarUser(UserCredential credentials, Name name, Email email)
        {
            _credentials = credentials;
            _firstName = name.FirstName;
            _lastName = name.LastName;
            _email = email;
            _isActive = true;
        }

        public static DaryaftyarUser Create(UserCredential credentials, Name name, Email email)
        {
            var user = new DaryaftyarUser(credentials, name, email);

            user.AddDomainEvent(new DaryaftyarUserCreatedEvent(user));

            return user;
        }

        public void BorrowBook(long bookId, DateTimePeriod borrowPeriod)
        {
            if (ActiveLoans.Count == 3)
                throw new DaryaftyarUserMaximumBooksBorrowedExceededException();

            _activeLoans.Add(Loan.Create(bookId, Id, borrowPeriod));

            AddDomainEvent(new DaryaftyarUserBorrowedBookEvent(Id, bookId, borrowPeriod));
        }

        public void ReturnBook(long bookId)
        {
            var bookLoanEntry = ActiveLoans.FirstOrDefault(x => x.BookId == bookId);

            if (bookLoanEntry is null)
                throw new DaryaftyarUserDoesNotHaveBookBorrowed(bookId);

            bookLoanEntry.Finish();

            AddDomainEvent(new DaryaftyarUserReturnedBookEvent(Id, bookId));
        }
    }
}
