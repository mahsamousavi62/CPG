using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Ipg.Queries;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.NeoBankServices.Queries;
using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.NeoBank;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;
using MediatR;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.QueryHandlers.NeoBankServices
{
    public class GetClientDirectDebitQueryHandler(INeoBankService neoBankService, IAggregateRepository<Transaction> transactionRepository) : IRequestHandler<GetClientDirectDebitQuery, Result<ClientDirectDebitResponse>>
    {
        private readonly INeoBankService neoBankService = neoBankService;
        private readonly IAggregateRepository<Transaction> transactionRepository = transactionRepository;

        public async Task<Result<ClientDirectDebitResponse>> Handle(GetClientDirectDebitQuery request, CancellationToken cancellationToken)
        {
            if (!Regex.IsMatch(request.Model.DestinationDepositNumber, "^\\d{4}/\\d{2}/\\d{3}/\\d{9}$"))
                throw new InvalidDepositNumberFormatException(request.Model.DestinationDepositNumber);

            try
            {
                var clientDirectDebitResponse = await neoBankService.ClientDirectDebit(request.Model);

                CharismaCardStatus status = CharismaCardStatus.Done;

                if (clientDirectDebitResponse.IsSuccess)
                {
                    var clientDirectDebit = clientDirectDebitResponse.Data;

                    Transaction transaction = Transaction.Create(new CreateTransactionModel
                    {
                        //DestinationDepositId = destinationDepositId,
                        //PaymentRequest = paymentRequest,
                        TransactionMethodType = Enums.TransactionType.CharismaCard,
                        Status = Enums.TransactionStatus.InPrgress,
                        
                       CharismaCardModel=new CharismaCardTransaction
                       {
                               TrackId = request.Model.TrackerId ?? null,
                               ProviderTrackId = clientDirectDebit.TranactionId,
                               ReferenceNumber = clientDirectDebit.ReferenceNumber,
                               Status = status
                       }
                    });
                    await transactionRepository.AddAsync(transaction);
                    await transactionRepository.SaveChangesAsync();

                }

                return clientDirectDebitResponse;

            }
            catch (DomainException exc)
            {
                return Result<ClientDirectDebitResponse>.Failure(new Error((exc as dynamic).Code, exc.Message));
            }
            catch (AppException exc)
            {
                return Result<ClientDirectDebitResponse>.Failure(new Error((exc as dynamic).Code, exc.Message));
            }
            catch (Exception)
            {
                return Result<ClientDirectDebitResponse>.Failure(new Error("1007000", GlobalResource.GetPaymentTicketUnexpectedError));
            }
        }



    }

}
