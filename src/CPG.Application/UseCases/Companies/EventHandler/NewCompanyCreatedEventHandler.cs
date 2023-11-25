using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.EventHandler;

public class NewCompanyCreatedEventHandler(IAggregateRepository<Company> repository) : INotificationHandler<NewCompanyCreatedEvent>
{
    public async Task Handle(NewCompanyCreatedEvent @event, CancellationToken cancellationToken)
    {
        //TODO: Handle New Company Create Event
    }
}
   

