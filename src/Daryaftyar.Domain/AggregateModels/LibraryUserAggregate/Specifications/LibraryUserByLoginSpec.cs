using Ardalis.Specification;

namespace Daryaftyar.Domain.AggregateModels.DaryaftyarUserAggregate.Specifications
{
    public sealed class DaryaftyarUserByLoginSpec : Specification<DaryaftyarUser>, ISingleResultSpecification
    {
        public DaryaftyarUserByLoginSpec(string login)
        {
            Query
                .Where(user => user.Credentials.Login == login);
        }
    }

    public sealed class DaryaftyarUserWithActiveLoansSpec : Specification<DaryaftyarUser>, ISingleResultSpecification
    {
        public DaryaftyarUserWithActiveLoansSpec(long id)
        {
            Query
                .Include(user => user.ActiveLoans)
                .Where(user => user.Id == id);
        }
    }
}