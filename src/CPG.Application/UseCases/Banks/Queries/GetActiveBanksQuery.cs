using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.Books.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Queries
{
    public class GetActiveBanksQuery : IRequest<IReadOnlyCollection<BankViewModel>>
    {
    }
}
