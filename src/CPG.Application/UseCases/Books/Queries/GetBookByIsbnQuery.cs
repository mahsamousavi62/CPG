using CPG.Application.UseCases.Books.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Books.Queries
{
    public class GetBookByIsbnQuery : IRequest<BookViewModel>
    {
        public string Isbn { get; }

        public GetBookByIsbnQuery(string isbn) 
            => Isbn = isbn;
    }
}
