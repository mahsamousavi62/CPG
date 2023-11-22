using CPG.Application.UseCases.Companies.ViewModels;
using MediatR;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQuery : IRequest<CompanyViewModel>
{
    public long CompanyId { get; }

    public GetCompanyQuery(long companyId) => CompanyId = companyId;
}
