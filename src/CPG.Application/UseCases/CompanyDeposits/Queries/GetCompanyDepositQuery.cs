using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetCompanyDepositQuery(long companyDepositId) : IRequest<Result<CompanyDepositViewModel>>
{
    public long CompanyDepositId { get; } = companyDepositId;
}
