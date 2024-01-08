using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetCompanyIPGDepositsQuery(long companyIPGId) : IRequest<Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    public long CompanyIPGId { get; } = companyIPGId;
}