using MediatR;

namespace CPG.Application.UseCases.Banks.Commands.DeleteBank
{
    public record DeleteBankCommand(int bankId) : IRequest;
}
