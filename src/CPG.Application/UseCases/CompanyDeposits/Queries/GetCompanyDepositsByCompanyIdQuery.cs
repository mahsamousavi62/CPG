using System.Collections.Generic;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Queries
{
    public class GetCompanyDepositsByCompanyIdQuery(long companyId) :IRequest<IReadOnlyList<CompanyDepositViewModel>>
    {
        public long CompanyId = companyId;
    }
}
