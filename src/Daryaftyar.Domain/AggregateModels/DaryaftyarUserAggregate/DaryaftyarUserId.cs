using Ardalis.GuardClauses;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate
{
    public record DaryaftyarUserId
    {
        public long Value { get; }

        public DaryaftyarUserId(long value)
        {
            Value = Guard.Against.NegativeOrZero(value, nameof(DaryaftyarUserId));
        }

        public static implicit operator long(DaryaftyarUserId id)
            => id.Value;

        public static implicit operator DaryaftyarUserId(long id)
            => new(id);
    }
}