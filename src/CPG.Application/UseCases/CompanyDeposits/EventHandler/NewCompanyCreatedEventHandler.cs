using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Events;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace  CPG.Application.UseCases.CompanyDeposits.Commands.EventHandler;

public class NewCompanyDepositCreatedEventHandler: INotificationHandler<NewCompanyDepositCreatedEvent>
{
    public async Task Handle(NewCompanyDepositCreatedEvent @event, CancellationToken cancellationToken)
    {
        //TODO: Handle New Company Create Event
    }
}
   

