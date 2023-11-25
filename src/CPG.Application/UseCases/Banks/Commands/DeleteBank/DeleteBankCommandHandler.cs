using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.DeleteBank;

public class DeleteBankCommandHandler(
    IAggregateRepository<CPGUser> CPGUserRepository,
    IAggregateRepository<Bank> bankRepository,
    ICurrentUser currentUser) : IRequestHandler<DeleteBankCommand>
{
    private readonly IAggregateRepository<CPGUser> _CPGUserRepository = CPGUserRepository;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(DeleteBankCommand command, CancellationToken cancellationToken)
    {
        var cpgUser = await _CPGUserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new CPGUserNotFoundException(_currentUser.UserId);

        var bank = await _bankRepository.GetByIdAsync(command.bankId, cancellationToken)
            ?? throw new BankNotFoundException(command.bankId);

        await _bankRepository.DeleteAsync(bank, cancellationToken);
    }
}
