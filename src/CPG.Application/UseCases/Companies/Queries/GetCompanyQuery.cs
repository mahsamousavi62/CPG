using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQuery(long companyId) : IRequest<Result<CompanyViewModel>>
{
    public long CompanyId { get; } = companyId;
}
