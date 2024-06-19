using CPG.Domain.AggregateModels.CompanyDepositAggregate;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class IbanInfo
{
    public string Iban { get; set; }
    public bool IsValid { get; set; }
    public CompanyDeposit Deposit { get; set; }
}
