using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetCompanyIPGDepositsQuery(long companyIPGId) : IRequest<IReadOnlyCollection<CompanyDepositViewModel>>
{
    public long CompanyIPGId { get; } = companyIPGId;
}