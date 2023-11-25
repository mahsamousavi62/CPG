using CPG.Application.UseCases.Companies.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries
{
    public class GetAllCompanyQuery:IRequest<IReadOnlyCollection<CompanyViewModel>>
    {
    }
}
