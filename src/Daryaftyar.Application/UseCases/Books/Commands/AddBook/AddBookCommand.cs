using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Commands.AddBook
{
    public record AddBookCommand(string Title, string Author, string Subject, string Isbn) : IRequest<long>;
}
