using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class GetPaymentTicketQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> companyDepositRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> companyIPGRepository,
    IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> companyRepository,
    IAuthenticationService authenticationService,
    ReadDbContext context) : IRequestHandler<GetPaymentTokenCommand, Result<PaymentTokenResponse>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> _companyIPGRepository = companyIPGRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyAggregate.Company> _companyRepository = companyRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<PaymentTokenResponse>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var nationalCode = await _authenticationService.GetDataFromClaim<string>("NationalCode") ?? throw new Exception("nationalCode is empty");

            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode));
            if (paymentRequest is null) { throw new PaymentRequestNotFoundByCodeException(); }
            if (!paymentRequest.Company.IsActive) { throw new PaymentTokenInactiveCompanyException(); }
            if (paymentRequest.UrlExpirationDateTime < DateTime.UtcNow) { throw new PaymentRequestCodeExpiredException(); }
            if (paymentRequest.IsUsed) { throw new PaymentRequestCodeIsUsedBeforeException(); }
            if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) { throw new PaymentRequestCodeInvalidStatusException(); }

            var companyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGIncludeProvider(request.PaymentToken.CompanyIPGId));
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.PaymentToken.CompanyIPGId); }
            if (!companyIpg.IsActive) { throw new PaymentTokenInactiveIPGException(); }
            if (!companyIpg.IPGType.IsActive) { throw new PaymentTokenInactiveIPGTypeException(); }
            if (!companyIpg.Provider.IsActive) { throw new PaymentTokenInactiveProviderException(); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var (status, result) = await ipg.GetPaymentTokenAsync(
                new PaymentTokenRequest
                {
                    ProviderData = companyIpg.ProviderData,
                    PaymentRequestAmount = paymentRequest.Amount,
                    IpgRedirectionMethodType = (Enums.IpgRedirectionMethodType)paymentRequest.Company.IpgRedirectionMethodType,
                    SiteAddress = paymentRequest.Company.SiteAddress,
                    IpgBaseUrl = companyIpg.Provider.IpgBaseUrl,
                    NationalCode = nationalCode
                });

            if (status == (short)HttpStatusCode.OK)
            {
                long destinationDepositId;
                if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationDepositIban))
                {
                    var companyDeposit = await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(paymentRequest.DestinationDepositIban));
                    if (companyDeposit is null) throw new Exception("CompanyDeposit not found!");
                    if (companyDeposit.CompanyId != paymentRequest.CompanyId) { throw new PaymentTokenDepositNotBelongsCompanyException(); }
                    if (!companyDeposit.IsActive) { throw new PaymentTokenInactiveDepositException(); }
                    if (!companyDeposit.Bank.IsActive) { throw new PaymentTokenInactiveBankException(); }
                    destinationDepositId = companyDeposit.Id;
                }
                else
                {
                    var tempcompanyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGByIpgDeposit(companyIpg.Id));

                    if (tempcompanyIpg.IPGDeposits.SingleOrDefault() is null)
                        throw new Exception("CompanyDeposit not found!");
                    destinationDepositId = tempcompanyIpg.IPGDeposits.SingleOrDefault().CompanyDepositId;
                }

                Transaction transaction = Transaction.Create(new CreateTransactionModel
                {
                    CompanyIPG = companyIpg,
                    DestinationDepositId = destinationDepositId,
                    PaymentRequest = paymentRequest,
                    Token = result.JsonBody.JsonStr.Params.RefID,
                    TrackId = result.TrackerId = result.TrackerId,
                    TransactionMethodType = Enums.TransactionType.IPG
                });
                await _transactionRepository.AddAsync(transaction);
                await _transactionRepository.SaveChangesAsync();
                PaymentRequest.Update(paymentRequest);
                await _paymentRequestRepository.UpdateAsync(paymentRequest);
                await _paymentRequestRepository.SaveChangesAsync();

                result.IpgRedirectionMethodType = (Enums.IpgRedirectionMethodType)paymentRequest.Company.IpgRedirectionMethodType;

                return Result<PaymentTokenResponse>.SuccessResult(result);
            }
            else
            {
                return Result<PaymentTokenResponse>.FailureResult(result, null);
            }
        }
        catch (Exception exc)
        {
            if (exc is DomainException || exc is CPG.Application.UseCases.Exceptions.ApplicationException)
                return Result<PaymentTokenResponse>.Failure(new Error((exc as dynamic).Code, exc.Message));
            else
                return Result<PaymentTokenResponse>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
        }
    }
}



