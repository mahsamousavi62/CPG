using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;

public record CreateCompanyDepositCommand(CreateCompanyDepositViewModel Model) : IRequest<long>;