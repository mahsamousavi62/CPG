using ApplicationException = CPG.Application.UseCases.Exceptions.ApplicationException;

namespace CPG.Application.UseCases.CPGUsers.Exceptions
{
    public class CPGUserNotFoundException : ApplicationException
    {
        public override string Code => "CPG_user_not_found";
        public long CPGUserId { get; }

        public CPGUserNotFoundException(long cpgUserId) : base($"CPG User with ID {cpgUserId} has not been found.")
            => CPGUserId = cpgUserId;
    }
}
