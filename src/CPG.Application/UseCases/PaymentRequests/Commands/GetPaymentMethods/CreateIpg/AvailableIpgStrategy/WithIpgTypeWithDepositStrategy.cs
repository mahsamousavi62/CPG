using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;

public class WithIpgTypeWithDepositStrategy : IAvailableIpgStrategy
{
    public List<CompanyIPG> GetAvailableIpg(Company company, List<PaymentRequestMethodIpgType> paymentRequestMethodIpgTypes, List<PaymentRequestMethodDeposit> paymentRequestMethodDeposits, List<CompanyIPG> companyIPGs)
    {
        var SuggestCompanyDeposits = paymentRequestMethodDeposits.Select(c => c.CompanyDepositId).ToList();
        var companyIpgDeposits = companyIPGs.SelectMany(i => i.IPGDeposits);

        var acceptComponyIpgDeposit = companyIpgDeposits.Where(cid => SuggestCompanyDeposits.Contains(cid.CompanyDepositId));


        var suggestIpgTypes = paymentRequestMethodIpgTypes.Select(c => c.IpgTypeId).ToList();
        var acceptCompanyIpg = suggestIpgTypes.Where(x => company.CompanyIPGs.Select(c => c.IPGTypeId).Contains(x));
        if (!acceptCompanyIpg.Any() && !acceptComponyIpgDeposit.Any())
        {
            return null;
        }
        var result = companyIPGs.Where(i => acceptCompanyIpg.Select(d => d).Contains(i.IPGTypeId) &&
                                              acceptComponyIpgDeposit.Select(d => d.CompanyIPGId).Contains(i.Id)).ToList();

        return result;
    }
}
