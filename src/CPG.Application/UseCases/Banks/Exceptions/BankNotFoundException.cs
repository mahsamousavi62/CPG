using CPG.Application.Shared.Resource;
using AppException = CPG.Application.UseCases.Exceptions.AppException;

namespace CPG.Application.UseCases.Banks.Exceptions;

public class BankNotFoundException(int bankId) : AppException(string.Format(GlobalResource.BankNotFound, bankId))
{
    public override string Code => "bank_not_found";
    public int BankId { get; } = bankId;
}