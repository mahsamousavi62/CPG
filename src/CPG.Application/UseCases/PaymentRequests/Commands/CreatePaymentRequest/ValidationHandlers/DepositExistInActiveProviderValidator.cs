using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class DepositExistInActiveProviderValidator<T> : ValidatorHandler<T>
    where T : DepositExistInIpgValidatorModel
{
    public override void Handle(T model)
    {
        var activeProviderCompanyIpgDeposits = model.Company.CompanyIPGs.Where(t => t.IsActive && t.Provider.IsActive)
                                                                        .SelectMany(t => t.IPGDeposits.Select(x => x.CompanyDeposit))
                                                                        .ToList();

        var existInActiveProvider = model.MethodData.Select(t => t.IbanInfoList.Where(t => t.Deposit.IsActive &&
                                                                                           t.Deposit.Bank.IsActive &&
                                                                                           t.Deposit.PaymentMethods.Select(x => x.MethodType)
                                                                                                                   .Contains(PaymentMethodType.InternetPaymentGateway))
                                                                                                                   .Any(t => activeProviderCompanyIpgDeposits.Contains(t.Deposit)));

        if (existInActiveProvider?.Any() is false)
            throw new PaymentRequestNoActiveProviderDepositForIbansException(model.Company.PersianName);
    }
}