using System.Collections.Generic;
using System.Linq;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Events;
using CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.CPGUserAggregate
{
    public class CPGUser : Entity<long>, IAggregateRoot
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

        internal CPGUser()
        {
            _activeLoans = new List<Loan>();
        }

        private CPGUser(UserCredential credentials, Name name, Email email)
        {
            _credentials = credentials;
            _firstName = name.FirstName;
            _lastName = name.LastName;
            _email = email;
            _isActive = true;
        }

        public static CPGUser Create(UserCredential credentials, Name name, Email email)
        {
            var user = new CPGUser(credentials, name, email);

            user.AddDomainEvent(new CPGUserCreatedEvent(user));

            return user;
        }

        public void BorrowBook(long bookId, DateTimePeriod borrowPeriod)
        {
            if (ActiveLoans.Count == 3)
                throw new CPGUserMaximumBooksBorrowedExceededException();

            _activeLoans.Add(Loan.Create(bookId, Id, borrowPeriod));

            AddDomainEvent(new CPGUserBorrowedBookEvent(Id, bookId, borrowPeriod));
        }

        public void ReturnBook(long bookId)
        {
            var bookLoanEntry = ActiveLoans.FirstOrDefault(x => x.BookId == bookId);

            if (bookLoanEntry is null)
                throw new CPGUserDoesNotHaveBookBorrowed(bookId);

            bookLoanEntry.Finish();

            AddDomainEvent(new CPGUserReturnedBookEvent(Id, bookId));
        }
    }
}
