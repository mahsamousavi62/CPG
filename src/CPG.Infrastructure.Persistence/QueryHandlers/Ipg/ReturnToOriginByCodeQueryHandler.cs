using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg
{
    public class ReturnToOriginByCodeQueryHandler(IApplicationSettingsRepository applicationSettingsRepository,
        IAggregateRepository<PaymentRequest> paymentRequestRepository) : IRequestHandler<ReturnToOriginByCodeQuery, string>
    {
        private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
        private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;

        public async Task<string> Handle(ReturnToOriginByCodeQuery request, CancellationToken cancellationToken)
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.model.Code));
            if (paymentRequest is null) throw new PaymentRequestCodeNotFoundException();
            return $"{paymentRequest.CallBackUrl}?code={paymentRequest.PaymentCode}&status={(Enums.PaymentStatus)paymentRequest.Status}";
        }
    }
}
