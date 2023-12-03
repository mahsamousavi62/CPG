using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.ActivateBank;

public class ActivateBankCommandHandler(IAggregateRepository<Bank> bankRepository, ICurrentUser currentUser) : IRequestHandler<ActivateBankCommand>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(ActivateBankCommand command, CancellationToken cancellationToken)
    {
        var bank = await _bankRepository.GetByIdAsync(command.BankId, cancellationToken)
                   ?? throw new BankNotFoundException(command.BankId);

        if (command.IsActive)
            bank.SetAsActive(_currentUser.UserId);
        else 
            bank.SetAsInactive(_currentUser.UserId);

        await _bankRepository.SaveChangesAsync(cancellationToken);
    }
}
