using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.Verify;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class VerifyTransactionQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    ReadDbContext context) : IRequestHandler<VerifyTransactionQuery, ResultData<VerifyTransactionResponse>>
{

    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<VerifyTransactionResponse>> Handle(VerifyTransactionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == request.VerifyTransaction.CompanyIPGId);
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.VerifyTransaction.CompanyIPGId); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.Verify(new VerifyTransactionRequest
            {
                ProviderData = companyIpg.ProviderData,
                ProviderTrackerId = 1,
            });

            return new ResultData<VerifyTransactionResponse>
            {
                OperationResult = Enums.OperationResult.Succeeded,
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ResultData<VerifyTransactionResponse>
            {
                OperationResult = Enums.OperationResult.Failed,
                Error = ex.Message
            };
        }
    }
}