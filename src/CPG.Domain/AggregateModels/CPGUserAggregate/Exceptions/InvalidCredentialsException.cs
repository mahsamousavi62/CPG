using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class InvalidCredentialsException : DomainException
    {
        public override string Code => "invalid_credentials";

        public InvalidCredentialsException(string message) : base(message)
        {
        }
    }
}