using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DepositExistInIpgValidator<T> : ValidatorHandler<T>
where T : DepositExistInIpgValidatorModel
{
    public override void Handle(T model)
    {
    }
}