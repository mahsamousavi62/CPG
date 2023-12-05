using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyPersianNameException(string name)
        : DomainException(string.Format(Resource.Empty_PersianName, name))
    {
        public override string Code => "empty_persianName";
    }
}
