using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DepositExistInActiveCompanyIpgValidator<T> : ValidatorHandler<T>
    where T : DepositExistInIpgValidatorModel
{
    public override void Handle(T model)
    {
        var activeComanyIpgDeposits = model.Company.CompanyIPGs.Where(t => t.IsActive &&
                                                                           t.IPGType.IsActive &&
                                                                           model.PaymentMethodConfig.IpgConfig.IpgTypeCode.Contains(t.IPGType.Code))
                                                               .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                               .ToList();

        var existInActiveComanyIpgDeposits = model.MethodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                                    t.Deposit.Bank.IsActive &&
                                                                                                    t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                                            .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                                                                            .Any(t => activeComanyIpgDeposits.Contains(t.Deposit)));

        if (existInActiveComanyIpgDeposits?.Any() is false)
            throw new PaymentRequestNoActiveCompanyIpgDepositForIpgCodeException(model.Company.PersianName);
    }
}