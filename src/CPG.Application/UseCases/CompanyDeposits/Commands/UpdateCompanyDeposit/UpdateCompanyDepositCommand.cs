using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.UpdateCompanyDeposit;

public class UpdateCompanyDepositCommand(UpdateCompanyDepositViewModel model) : IRequest<Result<Unit>>
{
    public UpdateCompanyDepositViewModel Model { get; } = model;
}
