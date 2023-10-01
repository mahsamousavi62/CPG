using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class CPGUserCreationException : DomainException
    {
        public override string Code => "cannot_create_CPG_user";

        public CPGUserCreationException(string message) : base(message)
        {
        }
    }
}
