using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.BankAggregate.Specifications
{
    public sealed class BankByIdSpec : Specification<Bank>, ISingleResultSpecification
    {
        public BankByIdSpec(int bankId)
        {
            Query
                .Where(bank => bank.Id == bankId);
        }
    }
}
