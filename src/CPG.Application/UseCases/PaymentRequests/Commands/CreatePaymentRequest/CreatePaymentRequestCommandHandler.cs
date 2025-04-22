using CPG.Application.Auth;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Specifications;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel.Interfaces;
using Mapster;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;


public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<IPGType> ipgTypeRepository,
    IAggregateRepository<Provider> providerRepository,
    IApplicationSettingsRepository applicationSettingsRepository,
    IAuthenticationService authenticationService,
    IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository,
    IAuthService authService
    ) : IRequestHandler<CreatePaymentRequestCommand, Result<PaymentRequestResponseViewModel>>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<IPGType> _ipgTypeRepository = ipgTypeRepository;
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;
    private readonly IAuthService _authService = authService;

    public async Task<Result<PaymentRequestResponseViewModel>> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var validationOutputData = await Validate(request.Model, cancellationToken);

            PaymentRequest paymentRequest = request.Model.Adapt<PaymentRequest>();

            var config = _authService.GetJwtConfig();
            var appConfig = await _applicationSettingsRepository.GetAllApplicationSettings();
            var clientId = await _authenticationService.GetClientId(config.Authority);

            var application = await _applicationRepository.GetBySpecAsync(new ApplicationByIdpClientId(clientId), cancellationToken);
            if (application == null)
                throw new PaymentRequestApplicationNotFoundException();
            if (!application.IsActive)
                throw new PaymentRequestApplicationIsInactiveException(application.PersianName, application.EnglishName);

            if (!ValidateUrl(request.Model.CallBackUrl))
                throw new PaymentRequestInvalidUrlPatternException();

            var validCallBackUrl = application.ApplicationCallbackUrls.Select(a => a.CallbackUrl);
            var compareUri = new Uri(request.Model.CallBackUrl, UriKind.Absolute);

            if (validCallBackUrl.All(url => !Uri.TryCreate(url, UriKind.Absolute, out var baseUri) ||
                !string.Equals(baseUri.Host, compareUri.Host, StringComparison.OrdinalIgnoreCase)))
            {
                throw new PaymentRequestInvalidCallbackUrlException(request.Model.CallBackUrl);
            }

            paymentRequest.ApplicationId = application.Id;
            paymentRequest.CompanyId = validationOutputData.Company.Id;

            var paymentRequestMethods = new List<PaymentRequestMethod>();
            if (validationOutputData.MethodDataList?.Any() is true)
            {
                foreach (var item in validationOutputData.MethodDataList)
                {
                    paymentRequestMethods.Add(PaymentRequestMethod.Create(item.MethodType, item.IPGTypeList?.Select(t => t.Id)?.ToArray(),
                        item.IbanInfoList?.Select(t => t.Deposit.Id)?.ToArray()));
                }
            }
            PaymentRequest.Create(paymentRequest, int.Parse(appConfig.ExpireTime), clientId, application.EnglishName, paymentRequestMethods);

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
        catch (Exception ex)
        {
            return Result<PaymentRequestResponseViewModel>.Failure(new Error("1001000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }

    private async Task<ConfigData> Validate(CreatePaymentRequestViewModel model, CancellationToken cancellationToken)
    {

        if (model.CompanyCode == 0 && model.CompanyId == 0)
        {
            throw new PaymentRequestCompanyCodeRequiredException();
        }

        var amount = new Amount(model.Amount);
        var callBackUrl = new Url(model.CallBackUrl);

        if (model.IsAnonymous && string.IsNullOrWhiteSpace(model.NationalCode))
        {
            throw new PaymentRequestNationalCodeRequiredException();
        }


        if (!string.IsNullOrWhiteSpace(model.NationalCode))
        {
            try
            {
                var nationalCode = new NationalCode(model.NationalCode);
            }
            catch (Exception exc)
            {
                throw new PaymentRequestInvalidNationalCodeException();
            }
        }

        if (model.CompanyCode == 0)
        {
            model.CompanyCode = (short)model.CompanyId;
        }
        var sameTrackerId = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByTrackerId(model.TrackerId));
        var trackIdValidator = new TrackIdValidator<PaymentRequest>();
        trackIdValidator.Handle(sameTrackerId);

        var company = await _companyRepository.GetBySpecAsync(new CompanyDataByCodeSpec(model.CompanyCode), cancellationToken);
        var companyValidator = new CompanyValidator<Company>();
        companyValidator.Handle(company);

        var result = new ConfigData
        {
            Company = company,
        };

        if (model.PaymentMethodConfig is null)
        {
            var nullPaymentMethodConfigValidator = new NullPaymentMethodConfigValidator<Company>();
            nullPaymentMethodConfigValidator.Handle(company);
        }
        else
        {
            var activeMethods = GetActiveMethods(model.PaymentMethodConfig);
            var activeMethodValidator = new ActiveMethodValidator<ActiveMethodValidatorModel>();
            activeMethodValidator.Handle(new ActiveMethodValidatorModel { Company = company, ActiveMethods = activeMethods });

            var destinationDeposits = GetDestinationIbans(model.PaymentMethodConfig);
            var destinationDepositValidator = new DestinationDepositValidator<DestinationDepositValidatorModel>();
            destinationDepositValidator.Handle(new DestinationDepositValidatorModel
            {
                ActiveMethods = activeMethods,
                Company = company,
                DestinationDeposits = destinationDeposits,
                PaymentMethodConfig = model.PaymentMethodConfig,
            });

            List<IPGType> ipgTypes = null;
            if (activeMethods.Contains(PaymentMethodType.InternetPaymentGateway))
            {
                if (model.PaymentMethodConfig.IpgConfig.IpgTypeCode?.Any(t => t != null) is true)
                {
                    ipgTypes = await _ipgTypeRepository.ListAsync(new IPGTypeByCodeSpec(model.PaymentMethodConfig.IpgConfig.IpgTypeCode.ToArray()));
                    var ipgTypeValidator = new IpgTypeValidator<List<IPGType>>();
                    ipgTypeValidator.Handle(ipgTypes);
                }

                if (destinationDeposits?.Any() is false)
                {
                    var ipgTypeDestinationDepositValidator = new IpgTypeDestinationDepositValidator<Company>();
                    ipgTypeDestinationDepositValidator.Handle(company);
                }
            }

            if (activeMethods.Contains(PaymentMethodType.DirectDebit))
            {
                var anyDirectDebitProvider = await _providerRepository.AnyAsync(new ProviderByPaymentMethodSpec(PaymentMethodType.DirectDebit));

                var directDebitValidator = new DirectDebitValidator<DirectDebitValidatorModel>();
                directDebitValidator.Handle(new DirectDebitValidatorModel { Company = company, AnyDirectDebitProvider = anyDirectDebitProvider });
            }

            if (activeMethods.Contains(PaymentMethodType.CharismaCard))
            {
                var deposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                                  t.Bank.IsActive &&
                                                                  t.PaymentMethods.Select(t => t.MethodType)
                                                                                  .Contains(PaymentMethodType.CharismaCard))
                                                      .ToList();

                var charismaCardtValidator = new CharismaCardValidator<List<CompanyDeposit>>();
                charismaCardtValidator.Handle(deposits);
            }

            var methodData = GetMethodData(model.PaymentMethodConfig, company, ipgTypes);
            var ibanValidator = new IbanValidator<List<MethodData>>();
            ibanValidator.Handle(methodData);

            var destinationDepositIbanValidator = new DestinationDepositIbanValidator<DestinationDepositIbanValidatorModel>();
            destinationDepositIbanValidator.Handle(new DestinationDepositIbanValidatorModel { Company = company, MethodData = methodData });

            if (activeMethods.Contains(PaymentMethodType.InternetPaymentGateway) && destinationDeposits?.Any() is true)
            {
                var depositExistInIpgValidator = new DepositExistInIpgValidator<DepositExistInIpgValidatorModel>();
                depositExistInIpgValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData });

                var depositExistInActiveIpgValidator = new DepositExistInActiveIpgValidator<DepositExistInIpgValidatorModel>();
                depositExistInActiveIpgValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData });

                var depositExistInActiveIpgTypeValidator = new DepositExistInActiveIpgTypeValidator<DepositExistInIpgValidatorModel>();
                depositExistInActiveIpgTypeValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData });

                var depositExistInActiveProviderValidator = new DepositExistInActiveProviderValidator<DepositExistInIpgValidatorModel>();
                depositExistInActiveProviderValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData });

                var depositExistInCompanyIpgValidator = new DepositExistInCompanyIpgValidator<DepositExistInIpgValidatorModel>();
                depositExistInCompanyIpgValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData });

                var depositExistInActiveCompanyIpgValidator = new DepositExistInActiveCompanyIpgValidator<DepositExistInIpgValidatorModel>();
                depositExistInActiveCompanyIpgValidator.Handle(new DepositExistInIpgValidatorModel { Company = company, MethodData = methodData, PaymentMethodConfig = model.PaymentMethodConfig });
            }

            result.MethodDataList = methodData;
        }
        return result;
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
        ibans.AddRange(paymentMethodConfig.IpgConfig.DestinationDepositIban.Where(t => !string.IsNullOrEmpty(t)));
        ibans.AddRange(paymentMethodConfig.PaymentReceiptConfig.DestinationDepositIban.Where(t => !string.IsNullOrEmpty(t)));
        if (!string.IsNullOrEmpty(paymentMethodConfig.DirectDebitConfig.DestinationDepositIban))
            ibans.Add(paymentMethodConfig.DirectDebitConfig.DestinationDepositIban);
        return ibans;
    }

    private List<MethodData> GetMethodData(PaymentMethodConfig paymentMethodConfig, Company company, List<IPGType> iPGTypes = null)
    {
        var ipgConfig = paymentMethodConfig.IpgConfig;
        var directDebitConfig = paymentMethodConfig.DirectDebitConfig;
        var charismaCardConfig = paymentMethodConfig.CharismaCardConfig;
        var recieptConfig = paymentMethodConfig.PaymentReceiptConfig;
        var response = new List<MethodData>();
        if (ipgConfig != null && ipgConfig.IsActive)
        {
            var ibanList = new List<IbanInfo>();
            foreach (var ipgIban in ipgConfig.DestinationDepositIban?.Where(t => !string.IsNullOrEmpty(t)))
            {
                ibanList.Add(new IbanInfo
                {
                    Deposit = company.CompanyDeposits.FirstOrDefault(t => t.Iban == ipgIban),
                    Iban = ipgIban,
                    IsValid = true
                });
            }
            response.Add(new MethodData { MethodType = PaymentMethodType.InternetPaymentGateway, IbanInfoList = ibanList, IPGTypeList = iPGTypes });
        }
        if (directDebitConfig != null && directDebitConfig.IsActive)
        {
            var ibanList = new List<IbanInfo>
            {
                new IbanInfo
                {
                    Deposit = company.CompanyDeposits.FirstOrDefault(t => t.Iban == directDebitConfig.DestinationDepositIban),
                    Iban = directDebitConfig.DestinationDepositIban,
                    IsValid = true
                }
            };
            response.Add(new MethodData { MethodType = PaymentMethodType.DirectDebit, IbanInfoList = !string.IsNullOrEmpty(directDebitConfig.DestinationDepositIban) ? ibanList : new List<IbanInfo>() });
        }
        if (recieptConfig != null && recieptConfig.IsActive)
        {
            var ibanList = new List<IbanInfo>();
            foreach (var ipgIban in recieptConfig.DestinationDepositIban?.Where(t => !string.IsNullOrEmpty(t)))
            {
                ibanList.Add(new IbanInfo
                {
                    Deposit = company.CompanyDeposits.FirstOrDefault(t => t.Iban == ipgIban),
                    Iban = ipgIban,
                    IsValid = true
                });
            }
            response.Add(new MethodData { MethodType = PaymentMethodType.PaymentReceipt, IbanInfoList = ibanList });
        }
        if (charismaCardConfig != null && charismaCardConfig.IsActive)
        {
            response.Add(new MethodData { MethodType = PaymentMethodType.CharismaCard, IbanInfoList = new List<IbanInfo>() });
        }
        return response;
    }

    private bool ValidateUrl(string url)
    {
        if (url.Length < 10 || url.Length > 2048)
            return false;

        if (!Regex.IsMatch(url, Constants.UrlPattern))
            return false;
        return true;
    }

    private class ConfigData
    {
        public Company Company { get; set; }
        public List<MethodData> MethodDataList { get; set; }
    }
}
