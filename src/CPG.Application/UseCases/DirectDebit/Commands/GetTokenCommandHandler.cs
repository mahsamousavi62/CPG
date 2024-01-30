using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

namespace CPG.Application.UseCases.DirectDebit.Commands;

public class GetTokenCommandHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<Provider> providerRepository,
    IAuthenticationService authenticationService
    ) : IRequestHandler<GetTokenCommand, Result<bool>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<bool>> Handle(GetTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var provider = await _providerRepository.GetByIdAsync(request.model.ProviderId);
            var ipg = _ipgFactory.GetInstance(provider.ProviderType);
            var tokenRequest = new PaymentTokenRequest();
            var tokenData = await ipg.GetPaymentTokenAsync(new PaymentTokenRequest { ProviderData = provider.ProviderData });
            switch (provider.ProviderType)
            {
                case Enums.ProviderType.Vandar:
                    {
                        provider.ProviderData = System.Text.Json.JsonSerializer.Serialize(new { Refresh_Token = tokenData.RefreshToken });
                        break;
                    }
                case Enums.ProviderType.AsanPardakht:
                    break;
                case Enums.ProviderType.Sep:
                    break;
                case Enums.ProviderType.Pec:
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {

        }
    }
}