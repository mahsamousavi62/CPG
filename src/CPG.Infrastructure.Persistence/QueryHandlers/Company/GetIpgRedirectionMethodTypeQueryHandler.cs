using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company
{
    public class GetIpgRedirectionMethodTypeQueryHandler:IRequestHandler<GetIpgRedirectionMethodTypeQuery, Dictionary<int, string>>
    {
        public async Task<Dictionary<int, string>> Handle(GetIpgRedirectionMethodTypeQuery request, CancellationToken cancellationToken)
   => await Task.FromResult(Enum.GetValues(typeof(Enums.IpgRedirectionMethodType))
       .Cast<Enums.IpgRedirectionMethodType>().ToDictionary(x => (int)x, x => x.ToString()));
    }
}
