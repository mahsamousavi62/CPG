using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;

namespace CPG.Application.UseCases.PaymentRequests.Exceptions;

public class TransactionNotFoundException(string trackId) : AppException(string.Format(GlobalResource.TransactionNotFound,trackId))
{
    public override string Code => "TransactionNotFound";
}