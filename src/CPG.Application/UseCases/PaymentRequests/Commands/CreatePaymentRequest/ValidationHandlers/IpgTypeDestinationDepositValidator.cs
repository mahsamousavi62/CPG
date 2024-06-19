using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System;
using System.Linq;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers;

internal class IpgTypeDestinationDepositValidator<T> : ValidatorHandler<T>
        where T : Company
{
    public override void Handle(T model)
    {
        var ipgDeposits = model.CompanyDeposits.Where(t => t.IsActive && t.Bank.IsActive && t.PaymentMethods.Select(t => t.MethodType).Contains(PaymentMethodType.InternetPaymentGateway));
        var companyDefaultIpgDeposits = model.CompanyIPGs.SelectMany(t => t.IPGDeposits.Where(t => t.IsDefault is true));
        var defaultIpgDeposits = ipgDeposits?.Where(x => companyDefaultIpgDeposits.Select(t => t.Id).Contains(x.Id));
        if (defaultIpgDeposits?.Any() is false)
            throw new PaymentRequestNoDefaultDepositException(model.PersianName);

        var companyActiveIpgDeposits = companyDefaultIpgDeposits.Select(x => new { x.Id, x.CompanyIPG }).Where(t => defaultIpgDeposits.Select(x => x.Id).Contains(t.Id));
        if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.IsActive is true) is false)
            throw new PaymentRequestNoActiveIpgException(model.PersianName);

        if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.IPGType.IsActive is true) is false)
            throw new PaymentRequestNoActiveIpgTypeException(model.PersianName);

        if (companyActiveIpgDeposits?.Any(t => t.CompanyIPG.Provider.IsActive is true) is false)
            throw new PaymentRequestNoActiveProviderException(model.PersianName);
    }
}