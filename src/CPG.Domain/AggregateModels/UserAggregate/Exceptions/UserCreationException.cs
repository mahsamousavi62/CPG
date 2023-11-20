using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class UserCreationException : DomainException
    {
        public override string Code => "cannot_create_user";

        public UserCreationException(string message) : base(message)
        {
        }
    }
}

