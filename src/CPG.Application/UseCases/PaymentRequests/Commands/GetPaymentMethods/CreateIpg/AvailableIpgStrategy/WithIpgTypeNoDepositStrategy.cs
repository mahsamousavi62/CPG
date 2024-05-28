using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;

public class WithIpgTypeNoDepositStrategy : IAvailableIpgStrategy
{
    public List<CompanyIPG> GetAvailableIpg(Company company, List<PaymentRequestMethodIpgType> paymentRequestMethodIpgTypes,
                                            List<PaymentRequestMethodDeposit> paymentRequestMethodDeposits,
                                            List<CompanyIPG> companyIPGs)
    {
        var suggestIpgTypes = paymentRequestMethodIpgTypes.Select(c => c.IpgTypeId).ToList();
        var companyIpg = suggestIpgTypes.Where(x => company.CompanyIPGs.Select(c => c.IPGTypeId).Contains(x));
        if (!companyIpg.Any())
        {
            return null;
        }
        var result = companyIPGs.Where(i => companyIpg.Select(d => d).Contains(i.Id)).ToList();
        return result;
    }
}
