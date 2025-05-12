using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class DestinationDepositIbanValidatorModel
{
    public List<MethodData> MethodData{ get; set; }
    public Company Company { get; set; }
}
