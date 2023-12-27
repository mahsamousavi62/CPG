using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class CreateTransactionModel
{
    public PaymentRequest PaymentRequest { get; set; }
    public CompanyIPG CompanyIPG { get; set; }
    public string TrackId { get; set; }
    public string Token { get; set; }
    public long DestinationDepositId { get; set; }
    public Enums.TransactionType TransactionMethodType { get; set; }
}
