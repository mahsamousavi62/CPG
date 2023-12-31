using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Ipg.Queries;

public class GetPaymentTransactionInfoQuery(PaymentTransactionViewModel model) : IRequest<ResultData<TransactionResultResponse>>
{
    public PaymentTransactionViewModel PaymentTransaction { get; set; } = model;
}

