using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidEnglishNameException : DomainException
    {
        public override string Code => "invalid_EnglishName";

        public InvalidEnglishNameException(string message) : base(message)
        {
        }
    }
}
