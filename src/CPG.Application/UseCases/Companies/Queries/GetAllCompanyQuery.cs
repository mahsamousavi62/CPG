using CPG.Application.UseCases.Companies.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetAllCompanyQuery:IRequest<IReadOnlyCollection<CompanyViewModel>>
{
}
