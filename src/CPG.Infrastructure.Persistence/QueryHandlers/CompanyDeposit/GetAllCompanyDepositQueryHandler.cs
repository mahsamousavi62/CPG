using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit
{
    public class GetAllCompanyDepositQueryHandler : IRequestHandler<GetAllCompanyDepositQuery, IReadOnlyCollection<CompanyDepositViewModel>>
    {
        public async Task<IReadOnlyCollection<CompanyDepositViewModel>> Handle(GetAllCompanyDepositQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
