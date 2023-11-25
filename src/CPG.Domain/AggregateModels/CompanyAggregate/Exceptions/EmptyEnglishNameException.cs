using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyEnglishNameException:DomainException
    {
        public override string Code => "empty_englishName";
        public EmptyEnglishNameException(string message) : base(message)
        {
        }

    }
}
