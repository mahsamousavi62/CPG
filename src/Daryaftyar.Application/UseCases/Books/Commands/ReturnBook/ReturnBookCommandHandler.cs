using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.Books.Exceptions;
using Daryaftyar.Application.UseCases.DaryaftyarUsers.Exceptions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Commands.ReturnBook
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand>
    {
        private readonly IAggregateRepository<DaryaftyarUser> _DaryaftyarUserRepository;
        private readonly IAggregateRepository<Book> _bookRepository;
        private readonly ICurrentUser _currentUser;

        public ReturnBookCommandHandler(
            IAggregateRepository<DaryaftyarUser> DaryaftyarUserRepository,
            IAggregateRepository<Book> bookRepository,
            ICurrentUser currentUser)
        {
            _DaryaftyarUserRepository = DaryaftyarUserRepository;
            _bookRepository = bookRepository;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(ReturnBookCommand command, CancellationToken cancellationToken)
        {
            var spec = new DaryaftyarUserWithActiveLoansSpec(_currentUser.UserId);
            var DaryaftyarUser = await _DaryaftyarUserRepository.GetBySpecAsync(spec, cancellationToken)
                              ?? throw new DaryaftyarUserNotFoundException(_currentUser.UserId);

            _ = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken)
                ?? throw new BookNotFoundException(command.BookId);

            DaryaftyarUser.ReturnBook(command.BookId);

            await _DaryaftyarUserRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}