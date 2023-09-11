using System;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Commands.BorrowBook
{
    public record BorrowBookCommand(long BookId, DateTime BorrowingEndDate) : IRequest;

}
