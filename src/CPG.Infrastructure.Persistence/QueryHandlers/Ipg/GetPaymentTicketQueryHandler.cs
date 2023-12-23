using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg
{
    public class GetPaymentTicketQueryHandler(IIpgFactory ipgFactory,
        IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
      ReadDbContext context)

        : IRequestHandler<GetPaymentTokenCommand, ResultData<PaymentTokenResponse>>
    {
        private readonly IIpgFactory _ipgFactory = ipgFactory;
        private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
        private readonly ReadDbContext _context = context;

        public async Task<ResultData<PaymentTokenResponse>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
        {
            var paymentRequest = await _paymentRequestRepository.GetByIdAsync(request.PaymentToken.PaymentRequestId);
            if (paymentRequest is null) { throw new PaymentRequestNotFoundException(request.PaymentToken.PaymentRequestId); }

            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == request.PaymentToken.CompanyIPGId);
            if (paymentRequest is null) { throw new CompanyIPGNotFoundException(request.PaymentToken.CompanyIPGId); }


            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.GetPaymentTokenAsync(new PaymentTokenRequest { ProviderData = companyIpg.ProviderData, PaymentRequestAmount = paymentRequest.Amount });
            if (result.OperationResult == Enums.OperationResult.Succeeded)
            {
                await _paymentRequestRepository.UpdateAsync(paymentRequest);
                paymentRequest.IsUsed = true;
                await _paymentRequestRepository.SaveChangesAsync();
            }
            return result;
        }
    }

}
