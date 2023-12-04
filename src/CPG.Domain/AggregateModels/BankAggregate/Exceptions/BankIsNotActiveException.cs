using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate.Exceptions;

public class BankIsNotActiveException : DomainException
{
    public override string Code => "bank_is_already_not_active";
    public int BankId { get; }

    public BankIsNotActiveException(int bankId) : base(string.Format(Resource.BankIsAlreadyNotActive, bankId))
       => BankId = bankId;
}
