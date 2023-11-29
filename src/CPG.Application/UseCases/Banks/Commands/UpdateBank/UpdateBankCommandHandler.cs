using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Application.UseCases.Banks.Exceptions;

namespace CPG.Application.UseCases.Banks.Commands.UpdateBank;

public class UpdateBankCommandHandler(IAggregateRepository<Bank> bankRepository, ICurrentUser currentUser) : IRequestHandler<UpdateBankCommand>
{
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(UpdateBankCommand command, CancellationToken cancellationToken)
    {
        var bank = await _bankRepository.GetByIdAsync(command.BankId, cancellationToken)
                   ?? throw new BankNotFoundException(command.BankId);

        bank.Update(command.IbanPrefix);

        await _bankRepository.SaveChangesAsync(cancellationToken);
    }
}
