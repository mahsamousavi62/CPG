using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Commands.ReturnBook
{
    public record ReturnBookCommand(long BookId) : IRequest;
}
