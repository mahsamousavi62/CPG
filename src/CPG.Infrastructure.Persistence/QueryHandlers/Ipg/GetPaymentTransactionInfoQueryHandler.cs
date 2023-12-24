using System;
using System.Threading.Tasks;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class GetPaymentTransactionInfoQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    ReadDbContext context) : IRequestHandler<GetPaymentTransactionInfoQuery, ResultData<TransactionResultResponse>>
{

    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<TransactionResultResponse>> Handle(GetPaymentTransactionInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == request.PaymentTransaction.CompanyIPGId);
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.PaymentTransaction.CompanyIPGId); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.GetTransactionResult(new TransactionResultRequest
            {
                ProviderData = companyIpg.ProviderData,
                LocalInvoiceId = request.PaymentTransaction.LocalInvoiceId
            });

            return new ResultData<TransactionResultResponse>
            {
                OperationResult = Enums.OperationResult.Succeeded,
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ResultData<TransactionResultResponse>
            {
                OperationResult = Enums.OperationResult.Failed,
                Error = ex.Message
            };
        }
    }
}
