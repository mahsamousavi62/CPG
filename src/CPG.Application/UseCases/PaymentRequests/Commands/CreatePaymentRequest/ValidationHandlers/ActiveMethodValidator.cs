using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class ActiveMethodValidator<T> : ValidatorHandler<T>
    where T : ActiveMethodValidatorModel
{
    public override void Handle(T model)
    {
        if (model.ActiveMethods?.Any() is false || model.ActiveMethods.Count == 0)
            throw new PaymentRequestNoMethodException();

        var notExistMethods = model.ActiveMethods.Where(t => !model.Company.PaymentMethods.Select(x => x.MethodType).Contains(t));
        if (notExistMethods?.Any() is true)
        {
            var methodTitles = notExistMethods.Select(t => t.ToString()).ToList();
            throw new PaymentRequestCompanyMethodsException(string.Join(',', methodTitles), model.Company.PersianName);
        }
    }
}
