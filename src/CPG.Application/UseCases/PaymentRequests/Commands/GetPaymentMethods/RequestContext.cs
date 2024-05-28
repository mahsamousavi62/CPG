using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.AggregateModels.TransactionAggregate;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class RequestContext
{
    public Company Company { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
    public ICurrentUser CurrentUser { get; set; }
    public IMinioProvider MinioProvider { get; set; }
    public INeoBankService NeoBankService { get; set; }
    public IAggregateRepository<DirectDebitGrant> GrantRepository { get; set; }
    public IAggregateRepository<Transaction> TransactionRepository { get; internal set; }
}
