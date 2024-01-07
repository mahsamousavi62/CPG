

using CPG.Application.UseCases.Ipg.Queries;
using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Transactions;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using System.Diagnostics;
using CPG.Application.UseCases.PaymentRequests.Exceptions;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class ReturnToOriginByTrackIdQueryHandler(
        IApplicationSettingsRepository applicationSettingsRepository,
        IAggregateRepository<PaymentRequest> paymentRequestRepository,
        IAggregateRepository<Domain.AggregateModels.TransactionAggregate.Transaction> transactionRepository
        ) : IRequestHandler<ReturnToOriginByTrackIdQuery, string>
{
    private readonly IAggregateRepository<Domain.AggregateModels.TransactionAggregate.Transaction> _transactionRepository = transactionRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;

    public async Task<string> Handle(ReturnToOriginByTrackIdQuery request, CancellationToken cancellationToken)
    {
      var transaction= await _transactionRepository.GetBySpecAsync(new TransactionByIPGTransactionTrackId(request.model.TrackId));
        if (transaction is  null) throw new TransactionNotFoundException(request.model.TrackId); 
        var paymentRequest = transaction.PaymentRequest;
        return $"{paymentRequest.CallBackUrl}?code={paymentRequest.PaymentCode}&status={(Enums.PaymentStatus)paymentRequest.Status}";
    }
}

