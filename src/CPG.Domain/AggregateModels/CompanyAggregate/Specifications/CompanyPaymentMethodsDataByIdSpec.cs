using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyAggregate.Specifications;

public class CompanyPaymentMethodsDataByIdSpec : Specification<Company>, ISingleResultSpecification<Company>
{
    public CompanyPaymentMethodsDataByIdSpec(long companyId)
    {
        Query.Include(a => a.PaymentMethods)            
            .Where(company => company.Id == companyId);
    }
}