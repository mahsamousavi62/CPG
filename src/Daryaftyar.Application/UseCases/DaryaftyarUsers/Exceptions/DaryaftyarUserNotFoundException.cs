using ApplicationException = Daryaftyar.Application.UseCases.Exceptions.ApplicationException;

namespace Daryaftyar.Application.UseCases.DaryaftyarUsers.Exceptions
{
    public class DaryaftyarUserNotFoundException : ApplicationException
    {
        public override string Code => "Daryaftyar_user_not_found";
        public long DaryaftyarUserId { get; }

        public DaryaftyarUserNotFoundException(long daryaftyarUserId) : base($"Daryaftyar User with ID {daryaftyarUserId} has not been found.")
            => DaryaftyarUserId = daryaftyarUserId;
    }
}
