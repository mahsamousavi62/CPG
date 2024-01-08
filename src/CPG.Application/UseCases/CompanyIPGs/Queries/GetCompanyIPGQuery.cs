using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetCompanyIPGQuery(long companyIPGId) : IRequest<Result<CompanyIPGViewModel>>
{
    public long CompanyIPGId { get; } = companyIPGId;
}