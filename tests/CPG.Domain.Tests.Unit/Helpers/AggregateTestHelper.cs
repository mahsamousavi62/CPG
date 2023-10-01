using System;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Tests.Base;

namespace CPG.Domain.Tests.Unit.Helpers
{
    public class AggregateTestHelper : TestBase
    {
        protected CPGUser GetValidCPGUserAggregate() => PrepareCPGUserAggregate();
        protected Book GetValidBookAggregate() => PrepareBookAggregate();
        protected static DateTimePeriod GetValidDateTimePeriod() => PrepareValidDateTimePeriod();
        protected Loan GetSampleLoanEntity() => PrepareSampleLoanEntity();


        protected string GetBookTitle => CreateString();
        protected string GetBookAuthor => CreateString();
        protected string GetBookSubject => CreateString();
        protected static Isbn GetIsbn => new("9783161484100");
        

        private CPGUser PrepareCPGUserAggregate()
        {
            return new CPGUser
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