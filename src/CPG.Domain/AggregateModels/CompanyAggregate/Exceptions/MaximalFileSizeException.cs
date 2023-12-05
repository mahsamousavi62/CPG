using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;

internal class MaximalFileSizeException(string message) : 
    DomainException(string.Format(Resource.MaximalFileSize, message))
{
    public override string Code => "MaximalLogoSize";
}
