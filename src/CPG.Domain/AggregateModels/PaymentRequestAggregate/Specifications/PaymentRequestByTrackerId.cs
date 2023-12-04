using Ardalis.Specification;
using CPG.Application.UseCases.CompanyDeposits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
