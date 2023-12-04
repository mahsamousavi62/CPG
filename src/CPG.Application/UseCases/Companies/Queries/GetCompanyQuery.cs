using CPG.Application.UseCases.Companies.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQuery(long companyId) : IRequest<CompanyViewModel>
{
    public long CompanyId { get; } = companyId;
}
