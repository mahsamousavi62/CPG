using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyEnglishNameException(string name) :
        DomainException(string.Format(Resource.Empty_EnglishName, name))
    {
        public override string Code => "empty_englishName";
    }
}
