using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetActiveCompanyIPGsQuery(long companyId) : IRequest<IReadOnlyCollection<CompanyIPGDataViewModel>>
{
    public long CompanyId { get; } = companyId;
}
