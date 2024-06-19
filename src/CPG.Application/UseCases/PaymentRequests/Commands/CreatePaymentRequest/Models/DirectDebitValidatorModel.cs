using CPG.Domain.AggregateModels.CompanyAggregate;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class DirectDebitValidatorModel
{
    public Company Company { get; set; }
    public bool AnyDirectDebitProvider { get; set; }    
}
