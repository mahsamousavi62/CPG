using MediatR;

namespace CPG.Application.UseCases.Books.Commands.ReturnBook
{
    public record ReturnBookCommand(long BookId) : IRequest;
}
