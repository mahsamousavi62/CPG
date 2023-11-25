using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Books.Exceptions;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Books.Commands.ReturnBook;

public class ReturnBookCommandHandler(
    IAggregateRepository<CPGUser> CPGUserRepository,
    IAggregateRepository<Book> bookRepository,
    ICurrentUser currentUser) : IRequestHandler<ReturnBookCommand>
{
    private readonly IAggregateRepository<CPGUser> _CPGUserRepository = CPGUserRepository;
    private readonly IAggregateRepository<Book> _bookRepository = bookRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(ReturnBookCommand command, CancellationToken cancellationToken)
    {
        var spec = new CPGUserWithActiveLoansSpec(_currentUser.UserId);
        var CPGUser = await _CPGUserRepository.GetBySpecAsync(spec, cancellationToken)
                          ?? throw new CPGUserNotFoundException(_currentUser.UserId);

        _ = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken)
            ?? throw new BookNotFoundException(command.BookId);

        CPGUser.ReturnBook(command.BookId);

        await _CPGUserRepository.SaveChangesAsync(cancellationToken);
    }
}