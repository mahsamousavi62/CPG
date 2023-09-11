using System;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.SharedKernel;
using Daryaftyar.Tests.Base;

namespace Daryaftyar.Domain.Tests.Unit.Helpers
{
    public class AggregateTestHelper : TestBase
    {
        protected DaryaftyarUser GetValidDaryaftyarUserAggregate() => PrepareDaryaftyarUserAggregate();
        protected Book GetValidBookAggregate() => PrepareBookAggregate();
        protected static DateTimePeriod GetValidDateTimePeriod() => PrepareValidDateTimePeriod();
        protected Loan GetSampleLoanEntity() => PrepareSampleLoanEntity();


        protected string GetBookTitle => CreateString();
        protected string GetBookAuthor => CreateString();
        protected string GetBookSubject => CreateString();
        protected static Isbn GetIsbn => new("9783161484100");
        

        private DaryaftyarUser PrepareDaryaftyarUserAggregate()
        {
            return new DaryaftyarUser
            {
                Id = CreateLong()
            };
        }

        private Book PrepareBookAggregate()
        {
            return new Book
            {
                Id = CreateLong(),
                _bookInformation = new BookInformation(GetBookTitle, GetBookAuthor, GetBookSubject, GetIsbn.Value),
                _inStock = true
            };
        }
        
        private static DateTimePeriod PrepareValidDateTimePeriod() 
            => DateTimePeriod.Create(DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        private Loan PrepareSampleLoanEntity()
            => Loan.Create(CreateLong(), CreateLong(), PrepareValidDateTimePeriod());
    }
}