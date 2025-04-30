using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;

public class PaymentRequestVerifyByTrackerId : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
{
    public PaymentRequestVerifyByTrackerId(string trackerId)
    {
        Query.Include(c => c.Company).Where(t => t.TrackerId == trackerId);
    }
}