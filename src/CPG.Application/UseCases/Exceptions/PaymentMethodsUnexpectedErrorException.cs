using CPG.Application.Shared.Resource;

namespace CPG.Application.UseCases.Exceptions;

internal class PaymentMethodsUnexpectedErrorException() : AppException(GlobalResource.UnexpectedError)
{
    public override string Code => "1006000";
}