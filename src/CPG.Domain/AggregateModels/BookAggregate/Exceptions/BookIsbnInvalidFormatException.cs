using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions;

public class BookIsbnInvalidFormatException(string isbn) : DomainException($"ISBN value has to be 10 or 13 length. Passed ISBN number is: {isbn}.")
{
    public override string Code => "invalid_isbn_format";
    public string Isbn { get; } = isbn;
}
