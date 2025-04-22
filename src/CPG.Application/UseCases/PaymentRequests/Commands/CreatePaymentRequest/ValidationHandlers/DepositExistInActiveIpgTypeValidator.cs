using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DepositExistInActiveIpgTypeValidator<T> : ValidatorHandler<T>
    where T : DepositExistInIpgValidatorModel
{
    public override void Handle(T model)
    {
        var activeIpgTypeCompanyIpgDeposits = model.Company.CompanyIPGs.Where(t => t.IsActive && t.IPGType.IsActive)
                                                                     .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                                     .ToList();
        var existInActiveIpgType = model.MethodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                          t.Deposit.Bank.IsActive &&
                                                                                          t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                                  .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                                                                  .Any(t => activeIpgTypeCompanyIpgDeposits.Contains(t.Deposit)));

        if (existInActiveIpgType?.Any() is false)
            throw new PaymentRequestNoActiveIpgTypeDepositForIbansException(model.Company.PersianName);
    }
}