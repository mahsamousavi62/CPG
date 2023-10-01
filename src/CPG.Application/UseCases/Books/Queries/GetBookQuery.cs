using CPG.Application.UseCases.Books.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Books.Queries
{
    public class GetBookQuery : IRequest<BookViewModel>
    {
        public long BookId { get; }

        public GetBookQuery(long bookId) => BookId = bookId;
    }
}
