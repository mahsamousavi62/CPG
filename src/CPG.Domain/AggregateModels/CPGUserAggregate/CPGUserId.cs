using Ardalis.GuardClauses;

namespace CPG.Domain.AggregateModels.CPGUserAggregate
{
    public record CPGUserId
    {
        public long Value { get; }

        public CPGUserId(long value)
        {
            Value = Guard.Against.NegativeOrZero(value, nameof(CPGUserId));
        }

        public static implicit operator long(CPGUserId id)
            => id.Value;

        public static implicit operator CPGUserId(long id)
            => new(id);
    }
}