using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.Ipg.Commands;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Specifications;
using CPG.Domain.AggregateModels.CompanyIPGAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Ipg;

public class GetPaymentTicketQueryHandler(IIpgFactory ipgFactory,
    IAggregateRepository<PaymentRequest> paymentRequestAggregateRepository,
    IAggregateRepository<Transaction> transactionRepository,
    IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> companyDepositRepository,

    IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG> companyIPGRepository,
    
    ReadDbContext context) : IRequestHandler<GetPaymentTokenCommand, ResultData<PaymentTokenResponse>>
{
    private readonly IIpgFactory _ipgFactory = ipgFactory;
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestAggregateRepository;
    private readonly IAggregateRepository<Domain.AggregateModels.CompanyIPGAggregate.CompanyIPG>
        _companyIPGRepository = companyIPGRepository;

    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
    private readonly IAggregateRepository<CPG.Application.UseCases.CompanyDeposits.CompanyDeposit> _companyDepositRepository = companyDepositRepository;
    private readonly ReadDbContext _context = context;

    public async Task<ResultData<PaymentTokenResponse>> Handle(GetPaymentTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = await _paymentRequestRepository.GetBySpecAsync(new PaymentRequestByCode(request.PaymentToken.PaymentRequestCode));
            if (paymentRequest is null) { throw new PaymentRequestNotFoundException(request.PaymentToken.PaymentRequestCode); }

            var companyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGIncludeProvider(request.PaymentToken.CompanyIPGId));
            if (companyIpg is null) { throw new CompanyIPGNotFoundException(request.PaymentToken.CompanyIPGId); }

            var ipg = _ipgFactory.GetInstance(Enums.ProviderType.AsanPardakht);
            var result = await ipg.GetPaymentTokenAsync(
                new PaymentTokenRequest
                {
                    ProviderData = companyIpg.ProviderData,
                    PaymentRequestAmount = paymentRequest.Amount,
                    IpgRedirectionMethodType = (Enums.IpgRedirectionMethodType)paymentRequest.Company.IpgRedirectionMethodType,
                    SiteAddress = paymentRequest.Company.SiteAddress,
                    IpgBaseUrl = companyIpg.Provider.IpgBaseUrl,
                });

            long destinationDepositId;
            if (!string.IsNullOrWhiteSpace(paymentRequest.DestinationIban)) 
            {
                var companyDeposit= await _companyDepositRepository.GetBySpecAsync(new CompanyDepositByIban(paymentRequest.DestinationIban));
              if(companyDeposit is null) throw new Exception("CompanyDeposit not found!");
                destinationDepositId = companyDeposit.Id;
            }
            else 
            {
                var tempcompanyIpg = await _companyIPGRepository.GetBySpecAsync(new CompanyIPGByIpgDeposit(companyIpg.Id));

                if (tempcompanyIpg.IPGDeposits.SingleOrDefault() is null)
                    throw new Exception("CompanyDeposit not found!");
                destinationDepositId= tempcompanyIpg.IPGDeposits.SingleOrDefault().CompanyDepositId;
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

            return new ResultData<PaymentTokenResponse>
            {
                OperationResult = Enums.OperationResult.Succeeded,
                Data = result,
            };
        }
        catch (Exception ex)
        {
            return new ResultData<PaymentTokenResponse>
            {
                OperationResult = Enums.OperationResult.Failed,
                Error = ex.Message
            };
        }
    }



}



