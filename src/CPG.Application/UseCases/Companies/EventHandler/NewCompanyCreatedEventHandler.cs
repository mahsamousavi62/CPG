using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Events;
using CPG.Domain.AggregateModels.CPGUserAggregate.Events;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.EventHandler;
public class NewCompanyCreatedEventHandler : INotificationHandler<NewCompanyCreatedEvent>
{
    private readonly IAggregateRepository<Company> _repository;

    public NewCompanyCreatedEventHandler(IAggregateRepository<Company> repository)
    {
        _repository = repository;
    }

    public async Task Handle(NewCompanyCreatedEvent @event, CancellationToken cancellationToken)
    {

    }
}
   

