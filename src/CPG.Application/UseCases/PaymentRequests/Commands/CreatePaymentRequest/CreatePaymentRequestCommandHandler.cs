using CPG.Application.Auth;
using CPG.Application.Shared.Resource;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
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
using Mapster;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;
using System.Reflection;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Company> companyRepository,
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
            var compareUri = new Uri(request.Model.CallBackUrl, UriKind.Absolute);

            if (validCallBackUrl.All(url => !Uri.TryCreate(url, UriKind.Absolute, out var baseUri) ||
                !string.Equals(baseUri.Host, compareUri.Host, StringComparison.OrdinalIgnoreCase)))
            {
                throw new PaymentRequestInvalidCallbackUrlException(request.Model.CallBackUrl);
            }

            paymentRequest.ApplicationId = application.Id;
            PaymentRequest.Create(paymentRequest, int.Parse(appConfig.ExpireTime), clientId, application.EnglishName);

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
        if (model.CompanyCode == 0 || model.PaymentMethodConfig is null ||
            (model.PaymentMethodConfig.IpgConfig is null && model.PaymentMethodConfig.DirectDebitConfig is null &&
             model.PaymentMethodConfig.CharismaCardConfig is null && model.PaymentMethodConfig.PaymentReceiptConfig is null))
            throw new PaymentRequestRequiredDataException();

        var amount = new Amount(model.Amount);
        var callBackUrl = new Url(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        var company = await _companyRepository.GetBySpecAsync(new CompanyDataByCodeSpec(model.CompanyCode));

        if (company is null)
            throw new PaymentRequestNoCompanyFoundException();

        if (!company.IsActive)
            throw new PamentRequestInactiveCompanyException();

        var activeMethods = GetActiveMethods(model.PaymentMethodConfig);

        if (activeMethods?.Any() is false || activeMethods.Count == 0)
            throw new PaymentRequestNoMethodException();

        var notExistMethods = activeMethods.Where(t => !company.PaymentMethods.Select(x => x.MethodType).Contains(t));
        if (notExistMethods?.Any() is true)
        {
            var methodTitles = notExistMethods.Select(t => t.ToString()).ToList();
            throw new PaymentRequestCompanyMethodsException(String.Join(',', methodTitles), company.PersianName);
        }

        var destinationDeposits = GetDestinationIbans(model.PaymentMethodConfig);

        if (destinationDeposits?.Any() is false && company.CompanyDeposits?.Any() is false && company.CompanyDeposits.All(t => t.IsActive is false))
            throw new PaymentRequestInactiveDepositsException(company.PersianName);

        var activeMethodDeposits = GetActiveMethodsDestinationDeposits(model.PaymentMethodConfig, company, activeMethods);
        var notExistDeposits = activeMethodDeposits.Where(t => t.Deposits?.Any() is false).Select(t => t.MethodType);
        if (notExistDeposits?.Any() is true)
            throw new PaymentRequestNotExistDepositsException(String.Join(',', notExistDeposits), company.PersianName);

        var inactiveBankMethods = activeMethodDeposits.Where(t => t.Deposits.All(x => x.Bank.IsActive is false)).Select(t => t.MethodType);
        if (destinationDeposits?.Any() is false && inactiveBankMethods?.Any() is true)
            throw new PaymentRequestInactiveDepositBanksException(String.Join(',', inactiveBankMethods), company.PersianName);

        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new PaymentRequestDuplicateTrackerIdException(sameTrackerId.TrackerId);
    }

    private List<PaymentMethodType> GetActiveMethods(PaymentMethodConfig paymentMethodConfig)
    {
        var activeMethods = new List<PaymentMethodType>();
        var ipgConfig = paymentMethodConfig.IpgConfig;
        var directDebitConfig = paymentMethodConfig.DirectDebitConfig;
        var charismaCardConfig = paymentMethodConfig.CharismaCardConfig;
        var recieptConfig = paymentMethodConfig.PaymentReceiptConfig;
        if (ipgConfig != null && ipgConfig.IsActive == true)
            activeMethods.Add(PaymentMethodType.InternetPaymentGateway);
        if (directDebitConfig != null && directDebitConfig.IsActive == true)
            activeMethods.Add(PaymentMethodType.DirectDebit);
        if (charismaCardConfig != null && charismaCardConfig.IsActive == true)
            activeMethods.Add(PaymentMethodType.CharismaCard);
        if (recieptConfig != null && recieptConfig.IsActive == true)
            activeMethods.Add(PaymentMethodType.PaymentReceipt);
        return activeMethods;
    }

    private List<string> GetDestinationIbans(PaymentMethodConfig paymentMethodConfig)
    {
        var ibans = new List<string>();
        ibans.AddRange(paymentMethodConfig.IpgConfig.DestinationDepositIban);
        ibans.AddRange(paymentMethodConfig.PaymentReceiptConfig.DestinationDepositIban);
        ibans.Add(paymentMethodConfig.DirectDebitConfig.DestinationDepositIban);
        return ibans;
    }

    private List<DepositData> GetActiveMethodsDestinationDeposits(PaymentMethodConfig paymentMethodConfig, Company company, List<PaymentMethodType> activeMethods)
    {
        var deposits = new List<DepositData>();

        foreach (var item in activeMethods)
        {
            switch (item)
            {
                case PaymentMethodType.InternetPaymentGateway:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.InternetPaymentGateway)).ToList(),
                            MethodType = PaymentMethodType.InternetPaymentGateway,
                        });
                        break;
                    }
                case PaymentMethodType.PaymentReceipt:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.PaymentReceipt)).ToList(),
                            MethodType = PaymentMethodType.PaymentReceipt,
                        });
                        break;
                    }
                case PaymentMethodType.DirectDebit:
                    {
                        deposits.Add(new DepositData
                        {
                            Deposits = company.CompanyDeposits.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.DirectDebit)).ToList(),
                            MethodType = PaymentMethodType.DirectDebit,
                        });
                        break;
                    }
            }
        }

        return deposits;
    }

    private class DepositData
    {
        public PaymentMethodType MethodType { get; set; }
        public List<CompanyDeposit> Deposits { get; set; }
    }
}
