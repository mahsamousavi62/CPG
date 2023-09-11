using Daryaftyar.Domain.Exceptions;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Exceptions
{
    public class InvalidCredentialsException : DomainException
    {
        public override string Code => "invalid_credentials";

        public InvalidCredentialsException(string message) : base(message)
        {
        }
    }
}