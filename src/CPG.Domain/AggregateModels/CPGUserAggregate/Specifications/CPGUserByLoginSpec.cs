using Ardalis.Specification;

namespace CPG.Domain.AggregateModels.CPGUserAggregate.Specifications;

public sealed class CPGUserByLoginSpec : Specification<CPGUser>, ISingleResultSpecification<CPGUser>
{
    public CPGUserByLoginSpec(string login)
    {
        Query
            .Where(user => user.Credentials.Login == login);
    }
}

public sealed class CPGUserWithActiveLoansSpec : Specification<CPGUser>, ISingleResultSpecification<CPGUser>
{
    public CPGUserWithActiveLoansSpec(long id)
    {
        Query
            .Include(user => user.ActiveLoans)
            .Where(user => user.Id == id);
    }
}