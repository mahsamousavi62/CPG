using System;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.SeedWork;
using Daryaftyar.Domain.SharedKernel.Exceptions;

namespace Daryaftyar.Domain.SharedKernel
{
    public class Loan : Entity<long>
    {
        private long _bookId;
        private long _userId;
        private DateTimePeriod _borrowPeriod;
        private bool _isActive;
        public DateTimePeriod BorrowPeriod => _borrowPeriod;
        public bool IsActive => _isActive;
        public long BookId => _bookId;
        
        
        private Loan()
        {
        }

        private Loan(long bookId, long userId, DateTimePeriod borrowPeriod)
        {
            _bookId = bookId;
            _userId = userId;
            _borrowPeriod = borrowPeriod;
            _isActive = true;
        }

        public static Loan Create(long bookId, long userId, DateTimePeriod borrowPeriod)
        {
            var loan = new Loan(bookId, userId, borrowPeriod);

            return loan;
        }

        internal void Finish()
        {
            if (!IsActive)
                throw new LoanNotActiveException(Id);

            _borrowPeriod = DateTimePeriod.Create(_borrowPeriod.StartDate, DateTime.UtcNow);
            _isActive = false;
        }
    }
}