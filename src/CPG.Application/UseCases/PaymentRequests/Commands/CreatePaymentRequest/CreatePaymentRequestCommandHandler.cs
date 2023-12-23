using CPG.Application.UseCases.Application.Exceptions;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository, IAggregateRepository<Company> companyRepository,
   IApplicationSettingsRepository applicationSettingsRepository, IAuthenticationService authenticationService,
    IAggregateRepository<CPG.Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository) : IRequestHandler<CreatePaymentRequestCommand, PaymentRequestResponseViewModel>
{

    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;

    public async Task<PaymentRequestResponseViewModel> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        await Validate(request.Model);

        PaymentRequest paymentRequest = request.Model.Adapt<PaymentRequest>();

        var config = await _applicationSettingsRepository.GetAllApplicationSettings();
        var clientId = await _authenticationService.GetClientId(config.Authority);

        //applicationIdentifier
        var application = await _applicationRepository.GetBySpecAsync(new ApplicationByIdpClientId(clientId));
        if (application == null)
            throw new ApplicationNotFoundException(application.Id);
        if (!application.IsActive)
            throw new ApplicationIsNotActiveException(application.Id);
        if (!application.ApplicationIdentifiers.SingleOrDefault(a => a.IdpClientId == clientId).IsActive)
            throw new IdpClientIdIsNotActiveException(clientId);


        paymentRequest.ApplicationId = application.Id;
        PaymentRequest.Create(paymentRequest, config.ExpireTime, clientId, application.EnglishName);

        await _paymentRequestRepository.AddAsync(paymentRequest);
        await _paymentRequestRepository.SaveChangesAsync();
        
            return new PaymentRequestResponseViewModel
        {
            ExpirationDateTime = paymentRequest.UrlExpirationDateTime,
            Code = paymentRequest.Code,
            PageUrl = $"{config.Payment_Gateway_URL_Prefix.TrimEnd('/')}?code={paymentRequest.Code}",
            Status = paymentRequest.Status
        };
    }

    private async Task Validate(CreatePaymentRequestViewModel model)
    {
        if ((!model.CompanyId.HasValue || model.CompanyId == 0) && string.IsNullOrEmpty(model.DestinationIban))
            throw new PaymentRequestRequiredDataException();

        //TODO: check applicationcallbackurl exsist

        //TODO:check callbackUrl
        //نحوه تشخیص تمامی روش های پرداختی مربوط به شرکتTODO:

        var amount = new Amount(model.Amount);
        var callBackUrl = new Url(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        if (model.CompanyId.HasValue && model.CompanyId != 0)
        {
            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(model.CompanyId.Value));

            if (company is null)
                throw new CompanyNotFoundException(model.CompanyId.Value);

            if (!company.IsActive)
                throw new CompanyIsNotActiveException(model.CompanyId.Value);

            if (company.CompanyDeposits.Count == 0)
                throw new CompanyHasNotCompanyDepositException(model.CompanyId.Value);

            if (!company.CompanyDeposits.All(cd => cd.IsActive))
                throw new AllCompanyDepositsIsInActiveException(model.CompanyId.Value);
        }

        if (!string.IsNullOrEmpty(model.DestinationIban))
        {
            var iban = new Iban(model.DestinationIban);

            var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(model.DestinationIban));
            if (companyDeposit is null)
                throw new PaymentRequestNotDefinedCompanyDepositException();
            if (!companyDeposit.IsActive)
                throw new CompanyDepositIsNotActiveException(companyDeposit.Id);
            if (!companyDeposit.Bank.IsActive)
                throw new BankIsNotActiveException(companyDeposit.Bank.Id);

            if ((model.CompanyId.HasValue && model.CompanyId != 0) && !string.IsNullOrEmpty(iban))
                if (companyDeposit.CompanyId != model.CompanyId)
                    throw new PaymentRequestNotMatchIbanAndCompanyException();

            if (!companyDeposit.Company.IsActive)
                throw new PaymentRequestIbanCompanyInActiveException();

            if (!model.CompanyId.HasValue || model.CompanyId == 0)
                model.CompanyId = companyDeposit.CompanyId;
        }
        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new PaymentRequestDuplicateTrackerIdException(sameTrackerId.TrackerId);

    }
}
