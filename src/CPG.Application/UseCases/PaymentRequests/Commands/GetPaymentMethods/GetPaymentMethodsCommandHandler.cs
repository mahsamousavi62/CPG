using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateCharismaCard;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateDirectDebit;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg;
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreatePaymentReceipt;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommandHandler(
    ICurrentUser user,
    IMinioProvider minioProvider,
    INeoBankService neoBankService,
    IAuthenticationService authenticationService,
    IAggregateRepository<Company> companyRepository,
    IAggregateRepository<DirectDebitGrant> grantRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<PaymentRequest> paymentRequestRepository) :
    IRequestHandler<GetPaymentMethodsCommand, Result<PaymentMethodsViewModel>>
{
    private readonly ICurrentUser _user = user;
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly INeoBankService _neoBankService = neoBankService;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAggregateRepository<DirectDebitGrant> _grantRepository = grantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;

    public async Task<Result<PaymentMethodsViewModel>> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestWithChildsByCode(request.ViewModel.PaymentCode));

        if (paymentRequest is null)
        {
            throw new PaymentRequestCodeNotFoundException();
        }

        if (paymentRequest.UrlExpirationDateTime < DateTime.Now)
        {
            throw new PaymentRequestCodeIsExpiredException();
        }

        if (paymentRequest.IsUsed)
        {
            throw new PaymentRequestCodeIsUsedException();
        }

        paymentRequest.Status = Enums.PaymentStatus.RedirectedToCpg;
        await _paymentRequestRepository.UpdateAsync(paymentRequest);

        Company company = null;
        List<PaymentMethodType> availablePaymentMethodTypes = null;
        var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);

        company = await _companyRepository.FirstOrDefaultAsync(new CompanyPaymentMethodsByIdSpec(paymentRequest.CompanyId), cancellationToken);

        if (string.IsNullOrEmpty(sub))
        {
            availablePaymentMethodTypes = new List<PaymentMethodType> { PaymentMethodType.InternetPaymentGateway };
        }
        else
        {
            availablePaymentMethodTypes = company?.PaymentMethods?.Select(p => p.MethodType).ToList();
        }

        GetPaymentMethodsHandler  ipgHandler = new CreateIpgHandler();
        GetPaymentMethodsHandler  directDebitHandler= new CreateDirectDebitHandler();
        GetPaymentMethodsHandler charismaCardHandler = new CreateCharismaCardHandler();
        GetPaymentMethodsHandler paymentReceiptHandler = new CreatePaymentReceiptHandler();
        ipgHandler.SetNextHandler(directDebitHandler);
        directDebitHandler.SetNextHandler(charismaCardHandler);
        charismaCardHandler.SetNextHandler(paymentReceiptHandler);
        var req = new RequestContext
        {
            Company = company,
            PaymentRequest = paymentRequest,
            MinioProvider = _minioProvider,
            NeoBankService = _neoBankService,
            GrantRepository = _grantRepository,
            CurrentUser = _user,
            TransactionRepository = _transactionRepository
        };
        
        var PaymentMethodsViewModel = new PaymentMethodsViewModel();

        foreach (var item in availablePaymentMethodTypes)
        {

            await ipgHandler.HandleRequset(item, req, PaymentMethodsViewModel);
        }

        return Result<PaymentMethodsViewModel>.SuccessResult(new PaymentMethodsViewModel
        {
            Amount = paymentRequest.Amount,
            PaymentCode = paymentRequest.PaymentCode,
            IPGs = PaymentMethodsViewModel.IPGs,
            DirectDebits = PaymentMethodsViewModel.DirectDebits,
            Receipt = PaymentMethodsViewModel.Receipt,
            CharismaCard = PaymentMethodsViewModel.CharismaCard,
            CompanyName = company?.PersianName,
        });
    }
}
