using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
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

        : IRequestHandler<GetPaymentTicketQuery, ResultData<PaymentTicketResponse>>
    {
        private readonly IIpgFactory _ipgFactory = ipgFactory;
        private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
        private readonly ReadDbContext _context = context;

        public async Task<ResultData<PaymentTicketResponse>> Handle(GetPaymentTicketQuery request, CancellationToken cancellationToken)
        {
            var paymentRequest = await _paymentRequestRepository.GetByIdAsync(request.PaymentTicketRequest.PaymentRequestId);
            if (paymentRequest is null) { throw new PaymentRequestNotFoundException(request.PaymentTicketRequest.PaymentRequestId); }

            var companyIpg = await _context.CompanyIPGReadModels.FirstOrDefaultAsync(t => t.Id == request.PaymentTicketRequest.CompanyIPGId);
            if (paymentRequest is null) { throw new CompanyIPGNotFoundException(request.PaymentTicketRequest.CompanyIPGId); }


            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.GetPaymentTicketAsync(request.PaymentTicketRequest);
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
