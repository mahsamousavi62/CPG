using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company;

public class GetIpgRedirectionMethodTypeQueryHandler : IRequestHandler<GetIpgRedirectionMethodTypeQuery, Result<Dictionary<int, string>>>
{
    public async Task<Result<Dictionary<int, string>>> Handle(GetIpgRedirectionMethodTypeQuery request, CancellationToken cancellationToken)
    {
        var data = await Task.FromResult(Enum.GetValues(typeof(Enums.IpgRedirectionMethodType))
            .Cast<Enums.IpgRedirectionMethodType>().ToDictionary(x => (int)x, x => x.ToString()));
        return Result<Dictionary<int, string>>.SuccessResult(data);
    }
}
