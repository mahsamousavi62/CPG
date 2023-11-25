using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.BookAggregate.Specifications;

public sealed class AvailableBooksSpec : Specification<Book>
{
    public AvailableBooksSpec()
    {
        Query
            .Where(book => book.InStock);
    }
}