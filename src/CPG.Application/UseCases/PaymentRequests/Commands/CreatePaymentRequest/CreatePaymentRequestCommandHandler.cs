using CPG.Application.Auth;
using CPG.Application.Shared.Resource;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
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
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate.Specifications;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Specifications;
using System.Text.RegularExpressions;

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
            if (request.Model.PaymentMethodConfig is null)
            {
                request.Model.PaymentMethodConfig = new PaymentMethodConfig
                {
                    CharismaCardConfig = new CharismaCardConfig { IsActive = true },
                    DirectDebitConfig = new DirectDebitConfig { IsActive = true },
                    IpgConfig = new IpgConfig { IsActive = true },
                    PaymentReceiptConfig = new PaymentReceiptConfig { IsActive = true },
                };
            }

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

            List<PaymentRequestMethod> paymentRequestMethods = null;
            if (validationOutputData.MethodDataList?.Any() is true)
            {
                foreach (var item in validationOutputData.MethodDataList)
                {
                    paymentRequestMethods.Add(PaymentRequestMethod.Create(item.MethodType, item.IPGTypeList.Select(t => t.Id).ToArray(),
                        item.IbanInfoList.Select(t => t.Deposit.Id).ToArray()));
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
        catch (Exception)
        {
            return Result<PaymentRequestResponseViewModel>.Failure(new Error("1001000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }

    private async Task<ConfigData> Validate(CreatePaymentRequestViewModel model, CancellationToken cancellationToken)
    {
        var amount = new Amount(model.Amount);
        var callBackUrl = new Url(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        var company = await _companyRepository.GetBySpecAsync(new CompanyDataByCodeSpec(model.CompanyCode), cancellationToken);

        if (company is null)
            throw new PaymentRequestNoCompanyFoundException();

        if (!company.IsActive)
            throw new PamentRequestInactiveCompanyException();

        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new PaymentRequestDuplicateTrackerIdException(sameTrackerId.TrackerId);

        List<MethodData> methodData = null;
        List<IPGType> ipgTypes = null;

        var activeMethods = GetActiveMethods(model.PaymentMethodConfig);

        if (activeMethods?.Any() is false || activeMethods.Count == 0)
            throw new PaymentRequestNoMethodException();

        var notExistMethods = activeMethods.Where(t => !company.PaymentMethods.Select(x => x.MethodType).Contains(t));
        if (notExistMethods?.Any() is true)
        {
            var methodTitles = notExistMethods.Select(t => t.ToString()).ToList();
            throw new PaymentRequestCompanyMethodsException(string.Join(',', methodTitles), company.PersianName);
        }

        var destinationDeposits = GetDestinationIbans(model.PaymentMethodConfig);

        if (destinationDeposits?.Any() is false && company.CompanyDeposits?.Any() is false && company.CompanyDeposits.All(t => t.IsActive is false))
            throw new PaymentRequestInactiveDepositsException(company.PersianName);

        var activeMethodDeposits = GetActiveMethodsDestinationDeposits(model.PaymentMethodConfig, company, activeMethods);
        var notExistDeposits = activeMethodDeposits.Where(t => t.Deposits?.Any() is false).Select(t => t.MethodType);
        if (notExistDeposits?.Any() is true)
            throw new PaymentRequestNotExistDepositsException(string.Join(',', notExistDeposits), company.PersianName);

        var inactiveBankMethods = activeMethodDeposits.Where(t => t.Deposits.All(x => x.Bank.IsActive is false)).Select(t => t.MethodType);
        if (destinationDeposits?.Any() is false && inactiveBankMethods?.Any() is true)
            throw new PaymentRequestInactiveDepositBanksException(string.Join(',', inactiveBankMethods), company.PersianName);

        if (activeMethods.Contains(PaymentMethodType.InternetPaymentGateway))
        {
            ipgTypes = await _ipgTypeRepository.ListAsync(new IPGTypeByCodeSpec(model.PaymentMethodConfig.IpgConfig.IpgTypeCode.ToArray()));
            if (ipgTypes?.Any() is false)
                throw new PaymentRequestNotExistIpgTypesException();

            if (ipgTypes.All(t => t.IsActive is false))
                throw new PaymentRequestInactiveIpgTypesException();

            if (destinationDeposits?.Any() is false)
            {
                var ipgDeposits = company.CompanyDeposits.Where(t => t.IsActive && t.Bank.IsActive && t.PaymentMethods.Select(t => t.MethodType).Contains(PaymentMethodType.InternetPaymentGateway));
                var companyDefaultIpgDeposits = company.CompanyIPGs.SelectMany(t => t.IPGDeposits.Where(t => t.IsDefault is true));
                var defaultIpgDeposits = ipgDeposits?.Where(x => companyDefaultIpgDeposits.Select(t => t.Id).Contains(x.Id));
                if (defaultIpgDeposits?.Any() is false)
                    throw new PaymentRequestNoDefaultDepositException(company.PersianName);

                var companyActiveIpgDeposits = companyDefaultIpgDeposits.Select(x => new { x.Id, x.CompanyIPG }).Where(t => defaultIpgDeposits.Select(x => x.Id).Contains(t.Id));
                if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.IsActive is true) is false)
                    throw new PaymentRequestNoActiveIpgException(company.PersianName);

                if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.IPGType.IsActive is true) is false)
                    throw new PaymentRequestNoActiveIpgTypeException(company.PersianName);

                if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.Provider.IsActive is true) is false)
                    throw new PaymentRequestNoActiveProviderException(company.PersianName);
            }
        }

        if (activeMethods.Contains(PaymentMethodType.DirectDebit))
        {
            var anyDirectDebitProvider = await _providerRepository.AnyAsync(new ProviderByPaymentMethodSpec(PaymentMethodType.DirectDebit));
            if (anyDirectDebitProvider is false)
                throw new PaymentRequestNoActiveDirectDebitProviderException();

            var deposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                              t.Bank.IsActive &&
                                                              t.PaymentMethods.Select(t => t.MethodType)
                                                                              .Contains(PaymentMethodType.DirectDebit));
            if (deposits?.Any(t => t.IsDefaultForDirectDebit is true) is false)
                throw new PaymentRequestNoDefaultDirectDebitDepositException(company.PersianName);
        }

        if (activeMethods.Contains(PaymentMethodType.CharismaCard))
        {
            var deposits = company.CompanyDeposits.Where(t => t.IsActive &&
                                                              t.Bank.IsActive &&
                                                              t.PaymentMethods.Select(t => t.MethodType)
                                                                              .Contains(PaymentMethodType.CharismaCard));

            if (deposits?.Any(t => t.IsDefaultForCharismaCard is true && t.Bank.IbanPrefix == "078") is false)
                throw new PaymentRequestNoDefaultCharismaCardDepositException(company.PersianName);
        }

        methodData = GetMethodData(model.PaymentMethodConfig, company, ipgTypes);
        var invalidIbanMethods = methodData?.Where(t => t.IbanInfoList.Any(x => x.IsValid is false));
        if (invalidIbanMethods?.Any() is false)
        {
            throw new PaymentRequestInvalidIbanException(string.Join(',', invalidIbanMethods.Select(t => t.MethodType.ToString())));
        }

        var noDepositFoundForIbans = methodData.Where(t => t.IbanInfoList.Any(x => x.Deposit == null));
        if (noDepositFoundForIbans?.Any() is true)
            throw new PaymentRequestNoDepositFoundForIbanException(string.Join(',', noDepositFoundForIbans.Select(t => t.MethodType.ToString())), company.PersianName);

        var allDepositsAreInactive = methodData.Where(t => t.IbanInfoList.All(x => x.Deposit.IsActive is false));
        if (allDepositsAreInactive?.Any() is true)
            throw new PaymentRequestAllDepositsAreInactiveException(string.Join(',', allDepositsAreInactive.Select(t => t.MethodType.ToString())));

        var allDepositBanksAreInactive = methodData.Where(t => t.IbanInfoList.All(x => x.Deposit.Bank.IsActive is false));
        if (allDepositBanksAreInactive?.Any() is true)
            throw new PaymentRequestAllDepositBanksAreInactiveException(string.Join(',', allDepositBanksAreInactive.Select(t => t.MethodType.ToString())));

        var allDepositsNotSupportMethod = methodData.Where(t => t.IbanInfoList.All(x => !x.Deposit.PaymentMethods.Select(q => q.MethodType).Contains(t.MethodType)));
        if (allDepositsNotSupportMethod?.Any() is true)
            throw new PaymentRequestAllDepositsNotSupportMethodException(string.Join(',', allDepositsNotSupportMethod.Select(t => t.MethodType.ToString())));

        if (activeMethods.Contains(PaymentMethodType.InternetPaymentGateway) && destinationDeposits?.Any() is true)
        {
            var companyIpgDeposits = company.CompanyIPGs.SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                        .ToList();
            var existInIpg = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                            t.Deposit.Bank.IsActive &&
                                                                            t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                    .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                .Any(t => companyIpgDeposits.Contains(t.Deposit)));

            if (existInIpg?.Any() is false)
                throw new PaymentRequestNoIpgDepositForIbansException(company.PersianName);

            var activeCompanyIpgDeposits = company.CompanyIPGs.Where(t => t.IsActive)
                                                              .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                              .ToList();
            var existInActiveIpg = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                  t.Deposit.Bank.IsActive &&
                                                                                  t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                          .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                      .Any(t => activeCompanyIpgDeposits.Contains(t.Deposit)));

            if (existInActiveIpg?.Any() is false)
                throw new PaymentRequestNoActiveIpgDepositForIbansException(company.PersianName);

            var activeIpgTypeCompanyIpgDeposits = company.CompanyIPGs.Where(t => t.IsActive && t.IPGType.IsActive)
                                                                     .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                                     .ToList();
            var existInActiveIpgType = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                      t.Deposit.Bank.IsActive &&
                                                                                      t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                              .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                          .Any(t => activeIpgTypeCompanyIpgDeposits.Contains(t.Deposit)));

            if (existInActiveIpgType?.Any() is false)
                throw new PaymentRequestNoActiveIpgTypeDepositForIbansException(company.PersianName);

            var activeProviderCompanyIpgDeposits = company.CompanyIPGs.Where(t => t.IsActive && t.Provider.IsActive)
                                                                      .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                                      .ToList();

            var existInActiveProvider = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                       t.Deposit.Bank.IsActive &&
                                                                                       t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                               .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                           .Any(t => activeProviderCompanyIpgDeposits.Contains(t.Deposit)));

            if (existInActiveProvider?.Any() is false)
                throw new PaymentRequestNoActiveProviderDepositForIbansException(company.PersianName);

            var comanyIpgDeposits = company.CompanyIPGs.Where(t => t.IsActive &&
                                                                   model.PaymentMethodConfig.IpgConfig.IpgTypeCode.Contains(t.IPGType.Code))
                                                       .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                       .ToList();

            var existInComanyIpgDeposits = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                          t.Deposit.Bank.IsActive &&
                                                                                          t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                                  .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                              .Any(t => comanyIpgDeposits.Contains(t.Deposit)));

            if (existInComanyIpgDeposits?.Any() is false)
                throw new PaymentRequestNoCompanyIpgDepositForIpgCodeException(company.PersianName);

            var activeComanyIpgDeposits = company.CompanyIPGs.Where(t => t.IsActive &&
                                                                         t.IPGType.IsActive &&
                                                                         model.PaymentMethodConfig.IpgConfig.IpgTypeCode.Contains(t.IPGType.Code))
                                                             .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                             .ToList();

            var existInActiveComanyIpgDeposits = methodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                                t.Deposit.Bank.IsActive &&
                                                                                                t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                                        .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                              .Any(t => activeComanyIpgDeposits.Contains(t.Deposit)));

            if (existInActiveComanyIpgDeposits?.Any() is false)
                throw new PaymentRequestNoActiveCompanyIpgDepositForIpgCodeException(company.PersianName);
        }

        return new ConfigData
        {
            Company = company,
            MethodDataList = methodData
        };
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

    private List<MethodData> GetMethodData(PaymentMethodConfig paymentMethodConfig, Company company, List<IPGType> iPGTypes = null)
    {
        var ipgConfig = paymentMethodConfig.IpgConfig;
        var directDebitConfig = paymentMethodConfig.DirectDebitConfig;
        var charismaCardConfig = paymentMethodConfig.CharismaCardConfig;
        var recieptConfig = paymentMethodConfig.PaymentReceiptConfig;
        var response = new List<MethodData>();
        if (ipgConfig != null)
        {
            var ibanList = new List<IbanInfo>();
            foreach (var ipgIban in ipgConfig.DestinationDepositIban)
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
        if (directDebitConfig != null)
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
            response.Add(new MethodData { MethodType = PaymentMethodType.DirectDebit, IbanInfoList = ibanList });
        }
        if (recieptConfig != null)
        {
            var ibanList = new List<IbanInfo>();
            foreach (var ipgIban in recieptConfig.DestinationDepositIban)
            {
                ibanList.Add(new IbanInfo
                {
                    Deposit = company.CompanyDeposits.FirstOrDefault(t => t.Iban == ipgIban),
                    Iban = ipgIban,
                    IsValid = true
                });
            }
            response.Add(new MethodData { MethodType = PaymentMethodType.InternetPaymentGateway, IbanInfoList = ibanList });
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

    private class DepositData
    {
        public PaymentMethodType MethodType { get; set; }
        public List<CompanyDeposit> Deposits { get; set; }
    }

    private class MethodData
    {
        public PaymentMethodType MethodType { get; set; }
        public List<IbanInfo> IbanInfoList { get; set; }
        public List<IPGType> IPGTypeList { get; set; }
    }

    private class IbanInfo
    {
        public string Iban { get; set; }
        public bool IsValid { get; set; }
        public CompanyDeposit Deposit { get; set; }
    }

    private class ConfigData
    {
        public Company Company { get; set; }
        public List<MethodData> MethodDataList { get; set; }
    }
}
