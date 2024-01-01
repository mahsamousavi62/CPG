using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class BankIsActiveException : DomainException
{
    public override string Code => "bank_is_already_active";
    public int BankId { get; }

    public BankIsActiveException(int bankId) : base(string.Format(Resource.BankIsAlreadyActive, bankId))
       => BankId = bankId;
}
