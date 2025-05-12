using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;

public class NoIpgTypeNoDepositStrategy : IAvailableIpgStrategy
{
    public List<CompanyIPG> GetAvailableIpg(Company company, List<PaymentRequestMethodIpgType> paymentRequestMethodIpgTypes,
                                            List<PaymentRequestMethodDeposit> paymentRequestMethodDeposits,
                                            List<CompanyIPG> companyIPGs)
    {
        var companyDefaultIpgDeposits = companyIPGs.SelectMany(t => t.IPGDeposits.Where(t => t.IsDefault));
        if (!companyDefaultIpgDeposits.Any())
            return null;

        var result = companyIPGs.Where(i => companyDefaultIpgDeposits.Select(d => d.CompanyIPGId).Contains(i.Id)).ToList();
        return result;
    }
}
