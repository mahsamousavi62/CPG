using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;

public class PaymentRequestByCodeOrTrackerId : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
{
    public PaymentRequestByCodeOrTrackerId(string code, string trackerId)
    {
        Query.Where(t => t.PaymentCode == code || t.TrackerId == trackerId);
    }
}