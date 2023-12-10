using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetCompanyIPGQuery(long companyIPGId) : IRequest<CompanyIPGViewModel>
{
    public long CompanyIPGId { get; } = companyIPGId;
}