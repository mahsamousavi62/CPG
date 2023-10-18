using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Banks.Exceptions
{
    public class BankNotFoundException : ApplicationException
    {
        public override string Code => "bank_not_found";
        public int BankId { get; }

        public BankNotFoundException(int bankId) : base($"Bank with ID {bankId} has not been found.")
            => BankId = bankId;
    }
}