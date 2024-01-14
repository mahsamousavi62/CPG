using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.Queries;

public class GetCompanyIPGsQuery(long companyId) : IRequest<Result<IReadOnlyCollection<CompanyIPGDataViewModel>>>
{
    public long CompanyId { get; } = companyId;
}