using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;

public interface IAvailableIpgStrategy
{
    List<CompanyIPG> GetAvailableIpg(Company company, List<PaymentRequestMethodIpgType> paymentRequestMethodIpgTypes,
                                     List<PaymentRequestMethodDeposit> paymentRequestMethodDeposits,
                                     List<CompanyIPG> companyIPGs);
}
