using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions
{
    public class InvalidEmailException : DomainException
    {
        public override string Code => "invalid_email_format";
        public string Email { get; }

        public InvalidEmailException(string email, string message) : base(message)
            => Email = email;
    }
}