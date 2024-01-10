using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;

public class PaymentRequestByCode : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
{
    public PaymentRequestByCode(string code)
    {
        Query.Where(t => t.PaymentCode == code)
            .Include(p => p.Company)
            .ThenInclude(c => c.ShaparakSetting)
            .Include(p => p.Application);
    }
}