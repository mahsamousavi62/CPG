using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetAllCompanyDepositQuery : IRequest<IReadOnlyCollection<CompanyDepositViewModel>>
{
}
