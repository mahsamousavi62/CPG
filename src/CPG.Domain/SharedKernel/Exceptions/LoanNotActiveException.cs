using CPG.Domain.Exceptions;

namespace CPG.Domain.SharedKernel.Exceptions;

public class LoanNotActiveException(long loanId) : DomainException($"Loan with ID {loanId} is not active.")
{
   public override string Code => "loan_is_not_active";
    public long LoanId { get; } = loanId;
}
