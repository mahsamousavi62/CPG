using CPG.Application.UseCases.CompanyDeposits.Commands.CreateCompanyDeposit;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.CompanyDeposits.Commands.SetAsDefaultForDD;

public record SetAsDefaultForDDCommand(SetAsDefaultForDDViewModel model) : IRequest<Result<bool>>;