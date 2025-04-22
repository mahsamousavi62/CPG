using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class CharismaCardValidator<T> : ValidatorHandler<T>
    where T : List<CompanyDeposit>
{
    public override void Handle(T model)
    {

    }
}