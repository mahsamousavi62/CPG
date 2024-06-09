using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;
internal class IpgTypeValidator<T> : ValidatorHandler<T>
        where T : List<IPGType>
{
    public override void Handle(T model)
    {
        if (model?.Any() is false)
            throw new PaymentRequestNotExistIpgTypesException();

        if (model.All(t => t.IsActive is false))
            throw new PaymentRequestInactiveIpgTypesException();
    }
}