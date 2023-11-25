using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Banks.Exceptions;

public class BankNotFoundException(int bankId) : ApplicationException($"Bank with ID {bankId} has not been found.")
{
    public override string Code => "bank_not_found";
    public int BankId { get; } = bankId;
}