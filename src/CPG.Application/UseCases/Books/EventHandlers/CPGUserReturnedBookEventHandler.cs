using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Books.Exceptions;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate.Events;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Books.EventHandlers
{
    public class CPGUserReturnedBookEventHandler : INotificationHandler<CPGUserReturnedBookEvent>
    {
        private readonly IAggregateRepository<Book> _bookRepository;

        public CPGUserReturnedBookEventHandler(IAggregateRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }
        
        public async Task Handle(CPGUserReturnedBookEvent @event, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(@event.BookId, cancellationToken)
                    ?? throw new BookNotFoundException(@event.BookId);

            book.SetAsAvailable(@event.CPGUserId);

            await _bookRepository.SaveChangesAsync(cancellationToken);
        }
    }
}