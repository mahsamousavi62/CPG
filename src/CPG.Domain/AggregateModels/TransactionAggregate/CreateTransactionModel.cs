using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.SharedKernel;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.TransactionAggregate;

public class CreateTransactionModel
{
    public PaymentRequest PaymentRequest { get; set; }    
    public long DestinationDepositId { get; set; }
    public TransactionType TransactionMethodType { get; set; }
    public TransactionStatus Status { get; set; }
    public CreateIPGTransactionModel IPGTransactionModel { get; set; }
    public CreateDDTransactionModel DDTransactionModel { get; set; }
}

public class CreateIPGTransactionModel
{
    public CompanyIPG CompanyIPG { get; set; }
    public string TrackId { get; set; }
    public string Token { get; set; }
    public short IpgVerificationTimeLimit { get; set; }
}

public class CreateDDTransactionModel
{
    public long GrantId { get; set; }
    public Enums.DirectDebitTransactionStatus Status { get; set; }
    public string TrackId { get; set; }
    public string ProviderTrackId { get; set; }
    public string ProviderData { get; set; }
}