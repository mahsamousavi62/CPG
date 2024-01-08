using System.Collections.Generic;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetCompanyDepositsByCompanyIdQuery(long companyId) :IRequest<Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    public long CompanyId = companyId;
}
