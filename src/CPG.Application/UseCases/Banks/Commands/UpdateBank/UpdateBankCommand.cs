using MediatR;

namespace CPG.Application.UseCases.Banks.Commands.UpdateBank;

public record UpdateBankCommand(int BankId, string IbanPrefix) : IRequest;
