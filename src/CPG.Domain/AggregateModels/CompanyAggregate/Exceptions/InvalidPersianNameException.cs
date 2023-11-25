using CPG.Domain.Exceptions;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Exceptions
{
    public class InvalidPersianNameException : DomainException
    {
        public override string Code => "invalid_persianName";
        public InvalidPersianNameException(string message) : base(message)
        {
        }

    }
}
