using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace CPG.Application.UseCases.Companies.Queries
{
    public class GetIpgRedirectionMethodTypeQuery:IRequest<Dictionary<int, string>>
    {
    }
}
