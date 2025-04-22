using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DestinationDepositIbanValidator<T> : ValidatorHandler<T>
where T : DestinationDepositIbanValidatorModel
{
    public override void Handle(T model)
    {
        var noDepositFoundForIbans = model.MethodData.Where(t => t.IbanInfoList.Any(x => x.Deposit == null));
        if (noDepositFoundForIbans?.Any() is true)
            throw new PaymentRequestNoDepositFoundForIbanException(string.Join(',', noDepositFoundForIbans.Select(t => t.MethodType.ToString())), model.Company.PersianName);

        var allDepositsAreInactive = model.MethodData.Where(t => t.IbanInfoList?.Any() is true && t.IbanInfoList.All(x => x.Deposit.IsActive is false));
        if (allDepositsAreInactive?.Any() is true)
            throw new PaymentRequestAllDepositsAreInactiveException(string.Join(',', allDepositsAreInactive.Select(t => t.MethodType.ToString())));

        var allDepositBanksAreInactive = model.MethodData.Where(t => t.IbanInfoList?.Any() is true && t.IbanInfoList.All(x => x.Deposit.Bank.IsActive is false));
        if (allDepositBanksAreInactive?.Any() is true)
            throw new PaymentRequestAllDepositBanksAreInactiveException(string.Join(',', allDepositBanksAreInactive.Select(t => t.MethodType.ToString())));

        var allDepositsNotSupportMethod = model.MethodData.Where(t => t.IbanInfoList?.Any() is true && t.IbanInfoList.All(x => !x.Deposit.PaymentMethods.Select(q => q.MethodType).Contains(t.MethodType)));
        if (allDepositsNotSupportMethod?.Any() is true)
            throw new PaymentRequestAllDepositsNotSupportMethodException(string.Join(',', allDepositsNotSupportMethod.Select(t => t.MethodType.ToString())));

    }
}
