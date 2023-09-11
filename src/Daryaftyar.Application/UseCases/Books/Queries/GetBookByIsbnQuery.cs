using Daryaftyar.Application.UseCases.Books.ViewModels;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Queries
{
    public class GetBookByIsbnQuery : IRequest<BookViewModel>
    {
        public string Isbn { get; }

        public GetBookByIsbnQuery(string isbn) 
            => Isbn = isbn;
    }
}
