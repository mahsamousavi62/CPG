using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.CPGUsers.Exceptions;

public class CPGUserNotFoundException(long cpgUserId) : ApplicationException($"CPG User with ID {cpgUserId} has not been found.")
{
    public override string Code => "CPG_user_not_found";
    public long CPGUserId { get; } = cpgUserId;
}
