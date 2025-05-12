using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;

public class PaymentRequestWithChildsByCode : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
{
    public PaymentRequestWithChildsByCode(string code)
    {
        Query.Where(t => t.PaymentCode == code)
            .Include(p => p.Company)
            .ThenInclude(c => c.ShaparakSetting)
            .Include(p => p.Application)
            .Include(a=>a.PaymentRequestMethods.Where(p=>p.IsActive))
            .ThenInclude(b=>b.PaymentRequestMethodIpgTypes.Where(p => p.IsActive))
            .Include(c=>c.PaymentRequestMethods.Where(p => p.IsActive))
            .ThenInclude(d=>d.PaymentRequestMethodDeposits.Where(p => p.IsActive));
    }
}