using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class InvalidMaxWithdrawalAmountPerDayException(decimal amount) : DomainException(Resource.InvalidMaxWithdrawalAmountPerDay)
{
    public override string Code => "invalid_max_withdrawal_amount_perDay";
}