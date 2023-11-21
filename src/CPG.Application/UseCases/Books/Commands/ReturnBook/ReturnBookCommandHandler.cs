using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Books.Exceptions;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Books.Commands.ReturnBook
{
    public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand>
    {
        private readonly IAggregateRepository<CPGUser> _CPGUserRepository;
        private readonly IAggregateRepository<Book> _bookRepository;
        private readonly ICurrentUser _currentUser;

        public ReturnBookCommandHandler(
            IAggregateRepository<CPGUser> CPGUserRepository,
            IAggregateRepository<Book> bookRepository,
            ICurrentUser currentUser)
        {
            _CPGUserRepository = CPGUserRepository;
            _bookRepository = bookRepository;
            _currentUser = currentUser;
        }

        public async Task Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            var spec = new CPGUserWithActiveLoansSpec(_currentUser.UserId);
            var CPGUser = await _CPGUserRepository.GetBySpecAsync(spec, cancellationToken)
                              ?? throw new CPGUserNotFoundException(_currentUser.UserId);

            _ = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken)
                ?? throw new BookNotFoundException(request.BookId);

            CPGUser.ReturnBook(request.BookId);

            await _CPGUserRepository.SaveChangesAsync(cancellationToken);
        }
    }
}