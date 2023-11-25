using Ardalis.GuardClauses;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class DirectDebitLimitation
{
    public decimal? Amount { get; }

    public decimal? DailyTransaction { get; set; }

    private DirectDebitLimitation()
    {
    }

    public DirectDebitLimitation(decimal? amount, decimal? dailyTransaction)
    {
        if (amount != null)
        {
            Guard.Against.NegativeOrZero((decimal)amount, nameof(amount));
        }
        if (dailyTransaction != null)
        {
            Guard.Against.NegativeOrZero((decimal)dailyTransaction, nameof(dailyTransaction));
        }
    }
}
