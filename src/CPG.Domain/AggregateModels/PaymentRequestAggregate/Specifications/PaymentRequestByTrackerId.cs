using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications
{
    public class PaymentRequestByTrackerId : Specification<PaymentRequest>, ISingleResultSpecification<PaymentRequest>
    {
        public PaymentRequestByTrackerId(string trackerId)
        {
            Query.Where(c => c.TrackerId == trackerId);
        }
    }
}
