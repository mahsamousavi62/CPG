using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Banks.Commands.UpdateBank;

public record UpdateBankCommand(UpdateBankViewModel model) : IRequest<Result<bool>>;
