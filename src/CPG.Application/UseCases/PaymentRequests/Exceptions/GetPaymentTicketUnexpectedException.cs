using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class GetPaymentTicketUnexpectedErrorException() : AppException(string.Format(GlobalResource.GetPaymentTicketUnexpectedError))
{
    public override string Code => "1007000";
}