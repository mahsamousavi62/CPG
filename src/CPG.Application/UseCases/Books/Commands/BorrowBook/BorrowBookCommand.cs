using System;
using MediatR;

namespace CPG.Application.UseCases.Books.Commands.BorrowBook
{
    public record BorrowBookCommand(long BookId, DateTime BorrowingEndDate) : IRequest;

}
