
using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;

public class Transaction : AuditableEntity<long>
{
    public long PaymentRquestId { get; set; }
    public int MyProperty { get; set; }
    public PaymentRequest PaymentRequest { get; set; }

}
