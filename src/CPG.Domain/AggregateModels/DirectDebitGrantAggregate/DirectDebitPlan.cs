using CPG.Domain.SeedWork;

namespace CPG.Domain.AggregateModels.DirectDebitGrantAggregate;

public class DirectDebitPlan : AuditableEntity<int>, IAggregateRoot
{
    public short DurationPerMonth { get; set; }
}
