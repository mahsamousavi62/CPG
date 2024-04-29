using CPG.Application.UseCases.Companies.Queries;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company;

public class GetCompanyPaymentMethodQueryHandler : IRequestHandler<GetCompanyPaymentMethodsQuery, Result<Dictionary<int, string>>>
{
    public async Task<Result<Dictionary<int, string>>> Handle(GetCompanyPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var data = await Task.FromResult(Enum.GetValues(typeof(Enums.PaymentMethodType))
         .Cast<Enums.PaymentMethodType>()
         .ToDictionary(x => (int)x, x => x.ToString()));
        return Result<Dictionary<int, string>>.SuccessResult(data);
    }
}