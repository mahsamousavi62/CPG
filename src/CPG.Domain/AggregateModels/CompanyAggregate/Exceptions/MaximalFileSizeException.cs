using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class MaximalFileSizeException : DomainException
{
    public override string Code => "MaximalFileSize";
    public MaximalFileSizeException(string message) : base(message)
    {
    }
}
