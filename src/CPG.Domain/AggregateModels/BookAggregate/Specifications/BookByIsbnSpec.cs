using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.BookAggregate.Specifications
{
    public sealed class BookByIsbnSpec : Specification<Book>, ISingleResultSpecification<Book>
    {
        public BookByIsbnSpec(Isbn isbn)
        {
            Query
                .Include("_loans")
                .Where(book => book.BookInformation.Isbn == isbn);
        }
    }
}