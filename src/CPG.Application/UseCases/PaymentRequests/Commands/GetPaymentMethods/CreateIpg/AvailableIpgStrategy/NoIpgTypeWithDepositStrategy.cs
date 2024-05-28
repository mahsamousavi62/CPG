using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;

public class NoIpgTypeWithDepositStrategy : IAvailableIpgStrategy
{
    public List<CompanyIPG> GetAvailableIpg(Company company, List<PaymentRequestMethodIpgType> paymentRequestMethodIpgTypes,
                                            List<PaymentRequestMethodDeposit> paymentRequestMethodDeposits,
                                            List<CompanyIPG> companyIPGs)
    {
        var SuggestCompanyDeposits = paymentRequestMethodDeposits.Select(c => c.CompanyDepositId).ToList();
        var companyIpgDeposits = companyIPGs.SelectMany(i => i.IPGDeposits);
        var acceptDeposits = companyIpgDeposits.Where(cid => SuggestCompanyDeposits.Contains(cid.Id));

        if (!acceptDeposits.Any())
        {
            return null;
        }
        var result = companyIPGs.Where(i => acceptDeposits.Select(c => c.CompanyIPGId).Contains(i.Id)).ToList();
        return null;
    }
}
