using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DirectDebitValidator<T> : ValidatorHandler<T>
    where T : DirectDebitValidatorModel
{
    public override void Handle(T model)
    {
        if (model.AnyDirectDebitProvider is false)
            throw new PaymentRequestNoActiveDirectDebitProviderException();

        var deposits = model.Company.CompanyDeposits.Where(t => t.IsActive &&
                                                          t.Bank.IsActive &&
                                                          t.PaymentMethods.Select(t => t.MethodType)
                                                                          .Contains(PaymentMethodType.DirectDebit));
        if (deposits?.Any(t => t.IsDefaultForDirectDebit is true) is false)
            throw new PaymentRequestNoDefaultDirectDebitDepositException(model.Company.PersianName);
    }
}