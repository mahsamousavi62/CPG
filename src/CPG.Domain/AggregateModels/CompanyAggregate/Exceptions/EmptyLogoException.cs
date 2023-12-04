using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyLogoException(string name) :
        DomainException(string.Format(Resource.Empty_Logo, name))
    {
        public override string Code => "empty_logo";
    }
}
