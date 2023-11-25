using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class EmptyPersianNameException:DomainException
    {
        public override string Code => "empt♂y_persianName";
        public EmptyPersianNameException(string message) : base(message)
        {
        }

    }
}
