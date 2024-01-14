using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits.Queries;

public class GetAllCompanyDepositQuery : IRequest<Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
}
