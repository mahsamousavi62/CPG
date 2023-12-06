using Ardalis.GuardClauses;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using Mapster;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository, IAggregateRepository<Company> companyRepository,
   IApplicationSettingsRepository applicationSettingsRepository, IAuthenticationService authenticationService) : IRequestHandler<CreatePaymentRequestCommand, PaymentRequestViewModel>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<PaymentRequestViewModel> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        var config = await _applicationSettingsRepository.GetAllApplicationSettings();
        var clientId = await _authenticationService.GetClientId(config.Authority);



        await Validate(request.Model);
        PaymentRequest paymentRequest = MapModel(request.Model);

        PaymentRequest.Create(paymentRequest, config.ExpireTime);
        await _paymentRequestRepository.AddAsync(paymentRequest);
        await _paymentRequestRepository.SaveChangesAsync();

        return new PaymentRequestViewModel
        {
            ExpirationDateTime = paymentRequest.UrlExpirationDateTime,
            Code = paymentRequest.Code,
            PageUrl = paymentRequest.CallBackUrl,
            Status = paymentRequest.Status
        };
    }

    private PaymentRequest MapModel(CreatePaymentRequestViewModel model)
    {
        TypeAdapterConfig<CreatePaymentRequestViewModel, PaymentRequest>.NewConfig();

        var paymentRequest = model.Adapt<PaymentRequest>();

        return paymentRequest;
    }

    private async Task Validate(CreatePaymentRequestViewModel model)
    {

       


        //TODO:check applicationId
        //TODO:check callbackUrl

        if (string.IsNullOrEmpty(model.DestinationIban))
            throw new Exception($"{nameof(model.DestinationIban)} is null or empty.");

        var iban = new Iban(model.DestinationIban);
        var amount = new Amount(model.Amount);
        var callBackUrl = new CallBackUrl(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(model.DestinationIban));

        if (companyDeposit.CompanyId != model.CompanyId)
            throw new Exception("Company ID does not match.");

        model.CompanyId ??= companyDeposit.CompanyId;

        var existingCompany = await _companyRepository.GetByIdAsync(model.CompanyId) ?? throw new Exception("Company with the specified ID already exists.");
        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new Exception("Payment request with the same Tracker ID already exists.");
    }
}
