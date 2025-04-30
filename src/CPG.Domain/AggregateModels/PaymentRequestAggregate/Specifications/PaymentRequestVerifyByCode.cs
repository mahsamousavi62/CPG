using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;

public class PaymentRequestVerifyByCode : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
{
    public PaymentRequestVerifyByCode(string code)
    {
        Query.Include(p => p.Company).Where(t => t.PaymentCode == code);
    }
}