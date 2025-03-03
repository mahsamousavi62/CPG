using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.BookAggregate.Exceptions
{
    public class NationalCodeInvalidFormatException(string nationalCode) : DomainException(Resource.NationalCodeInvalidFormat)
    {
        public override string Code => "1001033";
        public string NationalCode { get; }
    }
}