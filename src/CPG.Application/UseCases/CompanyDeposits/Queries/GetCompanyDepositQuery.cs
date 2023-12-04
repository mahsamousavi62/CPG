using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetCompanyDepositQuery(long companyDepositId) : IRequest<CompanyDepositViewModel>
{
    public long CompanyDepositId { get; } = companyDepositId;
}
