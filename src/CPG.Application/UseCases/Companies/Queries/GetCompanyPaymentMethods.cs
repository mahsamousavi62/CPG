using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries
{
    public class GetCompanyPaymentMethodsQuery : IRequest<Dictionary<int, string>>
    {
    }
  
}
