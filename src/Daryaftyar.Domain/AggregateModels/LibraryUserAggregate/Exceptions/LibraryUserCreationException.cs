using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions
{
    public class DaryaftyarUserCreationException : DomainException
    {
        public override string Code => "cannot_create_Daryaftyar_user";

        public DaryaftyarUserCreationException(string message) : base(message)
        {
        }
    }
}
