using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.Books.Exceptions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.EventHandlers
{
    public class DaryaftyarUserBorrowedBookEventHandler : INotificationHandler<DaryaftyarUserBorrowedBookEvent>
    {
        private readonly IAggregateRepository<Book> _bookRepository;

        public DaryaftyarUserBorrowedBookEventHandler(IAggregateRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        
        public async Task Handle(DaryaftyarUserBorrowedBookEvent @event, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(@event.BookId, cancellationToken) 
                       ?? throw new BookNotFoundException(@event.BookId);

            book.SetAsNotAvailable(@event.DaryaftyarUserId, @event.BorrowPeriod);

            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}