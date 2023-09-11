using System.Threading;
using System.Threading.Tasks;
using Daryaftyar.Application.UseCases.Books.Exceptions;
using Daryaftyar.Domain.AggregateModels.BookAggregate;
using Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Events;
using Daryaftyar.Domain.SharedKernel;
using MediatR;

namespace Daryaftyar.Application.UseCases.Books.EventHandlers
{
    public class DaryaftyarUserReturnedBookEventHandler : INotificationHandler<DaryaftyarUserReturnedBookEvent>
    {
        private readonly IAggregateRepository<Book> _bookRepository;

        public DaryaftyarUserReturnedBookEventHandler(IAggregateRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        
        public async Task Handle(DaryaftyarUserReturnedBookEvent @event, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(@event.BookId, cancellationToken)
                    ?? throw new BookNotFoundException(@event.BookId);

            book.SetAsAvailable(@event.DaryaftyarUserId);

            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}