using CPG.Application.UseCases.Books.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Books.Queries;

public class GetBookByIsbnQuery(string isbn) : IRequest<BookViewModel>
{
    public string Isbn { get; } = isbn;
}
