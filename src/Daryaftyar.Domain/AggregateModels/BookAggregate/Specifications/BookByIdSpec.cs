using Ardalis.Specification;

namespace Daryaftyar.Domain.AggregateModels.BookAggregate.Specifications
{
    public sealed class BookByIdSpec : Specification<Book>, ISingleResultSpecification
    {
        public BookByIdSpec(long bookId)
        {
            Query
                .Include("_loans")
                .Where(book => book.Id == bookId);
        }
    }
}