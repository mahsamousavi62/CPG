using ApplicationException = Daryaftyar.Application.UseCases.Exceptions.ApplicationException;

namespace Daryaftyar.Application.UseCases.DaryaftyarUsers.Exceptions
{
    public class DaryaftyarUserNotFoundException : ApplicationException
    {
        public override string Code => "Daryaftyar_user_not_found";
        public long DaryaftyarUserId { get; }

        public DaryaftyarUserNotFoundException(long DaryaftyarUserId) : base($"Daryaftyar User with ID {DaryaftyarUserId} has not been found.") 
            => DaryaftyarUserId = DaryaftyarUserId;
    }
}
