using CPG.Application.UseCases.Books.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Books.Queries;

public class GetBookQuery(long bookId) : IRequest<BookViewModel>
{
    public long BookId { get; } = bookId;
}
