using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Books.Commands.AddBook;

public class AddBookCommandHandler(
    IAggregateRepository<CPGUser> CPGUserRepository,
    IAggregateRepository<Book> bookRepository,
    ICurrentUser currentUser) : IRequestHandler<AddBookCommand, long>
{
    private readonly IAggregateRepository<CPGUser> _CPGUserRepository = CPGUserRepository;
    private readonly IAggregateRepository<Book> _bookRepository = bookRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<long> Handle(AddBookCommand command, CancellationToken cancellationToken)
    {
        var CPGUser = await _CPGUserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new CPGUserNotFoundException(_currentUser.UserId);

        var book = Book.Register(command.Title, command.Author, command.Subject, command.Isbn, CPGUser.Id);

        await _bookRepository.AddAsync(book, cancellationToken);

        return book.Id;
    }
}