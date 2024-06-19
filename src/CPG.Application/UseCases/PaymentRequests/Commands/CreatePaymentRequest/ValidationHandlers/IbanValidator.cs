using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class IbanValidator<T> : ValidatorHandler<T>
    where T : List<MethodData>
{
    public override void Handle(T model)
    {
        var invalidIbanMethods = model?.Where(t => t.IbanInfoList.Any(x => x.IsValid is false));
        if (invalidIbanMethods?.Any() is true)
        {
            throw new PaymentRequestInvalidIbanException(string.Join(',', invalidIbanMethods.Select(t => t.MethodType.ToString())));
        }
    }
}
