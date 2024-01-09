using CPG.Domain.SharedKernel;
using MediatR;

namespace CPG.Application.UseCases.Banks.Commands.ActivateBank;

public record ActivateBankCommand(int BankId, bool IsActive) : IRequest<Result<bool>>;
