using CPG.Application.Shared.Resource;
using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.Banks.Exceptions;

public class BankNotFoundException(int bankId) : ApplicationException(string.Format(GlobalResource.BankNotFound, bankId))
{
    public override string Code => "bank_not_found";
    public int BankId { get; } = bankId;
}