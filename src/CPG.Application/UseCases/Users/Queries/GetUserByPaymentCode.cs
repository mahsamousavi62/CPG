
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Users.Queries;

public class GetUserByPaymentCodeQuery(string paymentCode) :IRequest<Result<UserViewModel>>
{
    public string PaymentCode { get; set; }=paymentCode;
}

