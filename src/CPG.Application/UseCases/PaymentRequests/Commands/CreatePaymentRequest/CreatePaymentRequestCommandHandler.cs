using CPG.Application.Auth;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using IdentityModel;
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
    IAggregateRepository<CPG.Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository,
    IAuthService authService
    ) : IRequestHandler<CreatePaymentRequestCommand, Result<PaymentRequestResponseViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;
    private readonly IAuthService _authService = authService;

    public async Task<Result<PaymentRequestResponseViewModel>> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await Validate(request.Model);

            PaymentRequest paymentRequest = request.Model.Adapt<PaymentRequest>();

            var config = _authService.GetJwtConfig();
            var appConfig = await _applicationSettingsRepository.GetAllApplicationSettings();
            var clientId = await _authenticationService.GetClientId(config.Authority);

            var application = await _applicationRepository.GetBySpecAsync(new ApplicationByIdpClientId(clientId), cancellationToken);
            if (application == null)
                throw new PaymentRequestApplicationNotFoundException();
            if (!application.IsActive)
                throw new PaymentRequestApplicationIsInactiveException(application.PersianName, application.EnglishName);

            var validCallBackUrl = application.ApplicationCallbackUrls.Select(a => a.CallbackUrl);

            foreach (var item in validCallBackUrl)
            {
                Uri baseUri = new Uri(item);
                Uri compareUri = new Uri(request.Model.CallBackUrl);
               if( !string.Equals(baseUri.Host, compareUri.Host, StringComparison.OrdinalIgnoreCase))
                    throw new PaymentRequestInvalidCallbackUrlException(request.Model.CallBackUrl);
            }
           

            paymentRequest.ApplicationId = application.Id;
            PaymentRequest.Create(paymentRequest, config.ExpireTime, clientId, application.EnglishName);

            await _paymentRequestRepository.AddAsync(paymentRequest, cancellationToken);
            await _paymentRequestRepository.SaveChangesAsync(cancellationToken);

            return Result<PaymentRequestResponseViewModel>.SuccessResult(new PaymentRequestResponseViewModel
            {
                ExpirationDateTime = paymentRequest.UrlExpirationDateTime.ToString("yyyy-MM-dd HH:mm:ss zzz"),
                PaymentCode = paymentRequest.PaymentCode,
                PageUrl = $"{appConfig.Payment_Gateway_URL_Prefix.TrimEnd('/')}?paymentCode={paymentRequest.PaymentCode}",
                Status = paymentRequest.Status
            });
        }
        catch (DomainException exc)
        {
            return Result<PaymentRequestResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<PaymentRequestResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<PaymentRequestResponseViewModel>.Failure(new Error("1001000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }

    private async Task Validate(CreatePaymentRequestViewModel model)
    {
        if ((!model.CompanyId.HasValue || model.CompanyId == 0) && string.IsNullOrEmpty(model.DestinationDepositIban))
            throw new PaymentRequestRequiredDataException();

        var amount = new Amount(model.Amount);
        var callBackUrl = new Url(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        if (model.CompanyId.HasValue && model.CompanyId != 0)
        {
            var company = await _companyRepository.GetBySpecAsync(new CompanyByIdSpec(model.CompanyId.Value));

            if (company is null)
                throw new PaymentRequestNoCompanyFoundException(model.CompanyId.Value);

            if (!company.IsActive)
                throw new PamentRequestInactiveCompanyException(model.CompanyId.Value);

            if (company.CompanyDeposits.Count == 0)
                throw new CompanyHasNotCompanyDepositException(model.CompanyId.Value);

            if (!company.CompanyDeposits.All(cd => cd.IsActive))
                throw new AllCompanyDepositsIsInActiveException(model.CompanyId.Value);
        }

        if (!string.IsNullOrEmpty(model.DestinationDepositIban))
        {
            var iban = new Iban(model.DestinationDepositIban);

            var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(model.DestinationDepositIban));
            if (companyDeposit is null)
                throw new PaymentRequestNotDefinedCompanyDepositException();

            if (!companyDeposit.IsActive)
                throw new CompanyDepositIsNotActiveException(companyDeposit.Id);

            if (!companyDeposit.Bank.IsActive)
                throw new PaymentRequestBankInactiveException();

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
