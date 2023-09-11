using System;
using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.Books.Exceptions;
using Daryaftyar.Application.UseCases.DaryaftyarUsers.Exceptions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications;
using Daryaftyar.Domain.SharedKernel;
using MediatR;
using BookNotFoundException = Daryaftyar.Application.UseCases.Books.Exceptions.BookNotFoundException;

namespace Daryaftyar.Application.UseCases.Books.Commands.BorrowBook
{
    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand>
    {
        private readonly IAggregateRepository<DaryaftyarUser> _DaryaftyarUserRepository;
        private readonly IAggregateRepository<Book> _bookRepository;
        private readonly ICurrentUser _currentUser;

        public BorrowBookCommandHandler(
            IAggregateRepository<DaryaftyarUser> DaryaftyarUserRepository,
            IAggregateRepository<Book> bookRepository,
            ICurrentUser currentUser)
        {
            _DaryaftyarUserRepository = DaryaftyarUserRepository;
            _bookRepository = bookRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(BorrowBookCommand command, CancellationToken cancellationToken)
        {
            var spec = new DaryaftyarUserWithActiveLoansSpec(_currentUser.UserId);
            var DaryaftyarUser = await _DaryaftyarUserRepository.GetBySpecAsync(spec, cancellationToken) 
                              ?? throw new DaryaftyarUserNotFoundException(_currentUser.UserId);

            // TODO: Get book from repo by its ISBN, not it directly
            var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken) 
                       ?? throw new BookNotFoundException(command.BookId);

            if (!book.InStock)
                throw new BookNotAvailableException(command.BookId);

            var dateTimePeriod = DateTimePeriod.Create(DateTime.UtcNow, command.BorrowingEndDate);

            DaryaftyarUser.BorrowBook(command.BookId, dateTimePeriod);

            await _DaryaftyarUserRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}