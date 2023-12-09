using Ardalis.GuardClauses;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
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
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;

public class CreatePaymentRequestCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<CompanyDeposit> companyDepositRepository, IAggregateRepository<Company> companyRepository,
   IApplicationSettingsRepository applicationSettingsRepository, IAuthenticationService authenticationService,
    IAggregateRepository<CPG.Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository) : IRequestHandler<CreatePaymentRequestCommand, PaymentRequestViewModel>
{

    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;

    public async Task<PaymentRequestViewModel> Handle(CreatePaymentRequestCommand request, CancellationToken cancellationToken)
    {
        if (request.Model.CompanyId == 0 || string.IsNullOrEmpty(request.Model.DestinationIban))
            throw new Exception("هر 2 پارامتر شناسه شرکت و شبای مقصد نمیتواند به صورت همزمان خالی باشد.");

        var config = await _applicationSettingsRepository.GetAllApplicationSettings();
        var clientId = await _authenticationService.GetClientId(config.Authority);

        var application = await _applicationRepository.GetBySpecAsync(new ApplicationByIdpClientId(clientId));
        if (application == null) { 
            throw new ApplicationNotFoundException(application.Id); 
        
        }

        //TODO: check applicationcallbackurl exsist
        await Validate(request.Model);
        PaymentRequest paymentRequest = request.Model.Adapt<PaymentRequest>();
        paymentRequest.ApplicationId = application.Id;
        PaymentRequest.Create(paymentRequest, config.ExpireTime, clientId, application.EnglishName);
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

    private async Task Validate(CreatePaymentRequestViewModel model)
    {
        //TODO:check callbackUrl
        //نحوه تشخیص تمامی روش های پرداختی مربوط به شرکتTODO:

        var iban = new Iban(model.DestinationIban);
        var amount = new Amount(model.Amount);
        var callBackUrl = new CallBackUrl(model.CallBackUrl);
        var nationalCode = new NationalCode(model.NationalCode);

        if (model.CompanyId.HasValue)
        {
            var company = await _companyRepository.GetByIdAsync(model.CompanyId);

            if (company is null)
                throw new CompanyNotFoundException(model.CompanyId.Value);

            if (!company.IsActive)
                throw new Exception("شرکت مورد نظر غیرفعال است");

            if (company.CompanyDeposits is null)
                throw new Exception("شرکت مورد نظر هیچ حسابی ندارد");

            if (company.CompanyDeposits.Where(cd=>cd.IsActive).Count() == 0)
                throw new Exception("تمامی حساب های شرکت مورد نظر غیرفعال هستند.");
        }
        
        var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(model.DestinationIban));
        if (companyDeposit is null)
            throw new Exception("به ازای شبای ارائه شده هیچ حسابی تعریف نشده است");
        if (!companyDeposit.IsActive)
            throw new Exception("حساب مورد نظر غیرفعال است.");
        if (companyDeposit.Bank.IsActive)
            throw new Exception("بانک حساب مورد نظر غیرفعال است");

        if (model.CompanyId.HasValue && !string.IsNullOrEmpty(iban))
            if (companyDeposit.CompanyId != model.CompanyId)
                throw new Exception("حساب ارائه شده با شرکت مورد نظر ارتباطی نداشته و برای این شرکت تعریف نشده است.");
        
        if (!companyDeposit.Company.IsActive)
            throw new Exception("شرکت مربوط به شبای ارائه شده غیرفعال است.");
        
        model.CompanyId ??= companyDeposit.CompanyId;

        var sameTrackerId = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByTrackerId(model.TrackerId));
        if (sameTrackerId != null)
            throw new Exception("Payment request with the same Tracker ID already exists.");
    }
}
