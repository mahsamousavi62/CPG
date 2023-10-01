using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Books.Exceptions;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;
using BookNotFoundException = CPG.Application.UseCases.Books.Exceptions.BookNotFoundException;

namespace CPG.Application.UseCases.Books.Commands.BorrowBook
{
    public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand>
    {
        private readonly IAggregateRepository<CPGUser> _CPGUserRepository;
        private readonly IAggregateRepository<Book> _bookRepository;
        private readonly ICurrentUser _currentUser;

        public BorrowBookCommandHandler(
            IAggregateRepository<CPGUser> CPGUserRepository,
            IAggregateRepository<Book> bookRepository,
            ICurrentUser currentUser)
        {
            _CPGUserRepository = CPGUserRepository;
            _bookRepository = bookRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(BorrowBookCommand command, CancellationToken cancellationToken)
        {
            var spec = new CPGUserWithActiveLoansSpec(_currentUser.UserId);
            var CPGUser = await _CPGUserRepository.GetBySpecAsync(spec, cancellationToken) 
                              ?? throw new CPGUserNotFoundException(_currentUser.UserId);

            // TODO: Get book from repo by its ISBN, not it directly
            var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken) 
                       ?? throw new BookNotFoundException(command.BookId);

            if (!book.InStock)
                throw new BookNotAvailableException(command.BookId);

            var dateTimePeriod = DateTimePeriod.Create(DateTime.UtcNow, command.BorrowingEndDate);

            CPGUser.BorrowBook(command.BookId, dateTimePeriod);

            await _CPGUserRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}