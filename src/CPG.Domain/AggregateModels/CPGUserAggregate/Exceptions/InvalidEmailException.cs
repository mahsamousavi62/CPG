using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Exceptions;

public class InvalidEmailException(string email, string message) : DomainException(message)
{
    public override string Code => "invalid_email_format";
    public string Email { get; } = email;
}