using CPG.Application.UseCases.Companies.Queries;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company
{
    public class GetCompanyPaymentMethodQueryHandler : IRequestHandler<GetCompanyPaymentMethodsQuery, Dictionary<int, string>>
    {
        public async Task<Dictionary<int, string>> Handle(GetCompanyPaymentMethodsQuery request, CancellationToken cancellationToken)
        => Enum.GetValues(typeof(Enums.CompanyPaymentMethodType)).Cast<Enums.CompanyPaymentMethodType>().ToDictionary(x => (int)x, x => x.ToString());
        
    }
}
