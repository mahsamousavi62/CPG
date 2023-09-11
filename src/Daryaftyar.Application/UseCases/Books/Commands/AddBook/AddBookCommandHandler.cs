using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.DaryaftyarUsers.Exceptions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.Commands.AddBook
{
    public class AddBookCommandHandler : IRequestHandler<AddBookCommand, long>
    {
        private readonly IAggregateRepository<DaryaftyarUser> _DaryaftyarUserRepository;
        private readonly IAggregateRepository<Book> _bookRepository;
        private readonly ICurrentUser _currentUser;

        public AddBookCommandHandler(
            IAggregateRepository<DaryaftyarUser> DaryaftyarUserRepository,
            IAggregateRepository<Book> bookRepository,
            ICurrentUser currentUser)
        {
            _DaryaftyarUserRepository = DaryaftyarUserRepository;
            _bookRepository = bookRepository;
            _currentUser = currentUser;
        }

        public async Task<long> Handle(AddBookCommand command, CancellationToken cancellationToken)
        {
            var DaryaftyarUser = await _DaryaftyarUserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new DaryaftyarUserNotFoundException(_currentUser.UserId);

            var book = Book.Register(command.Title, command.Author, command.Subject, command.Isbn, DaryaftyarUser.Id);

            await _bookRepository.AddAsync(book, cancellationToken);

            return book.Id;
        }
    }
}