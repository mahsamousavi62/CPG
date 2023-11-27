using CPG.Application.UseCases.Users.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.CreateBank;

public class CreateBankCommandHandler(
    IAggregateRepository<User> UserRepository,
    IAggregateRepository<Bank> bankRepository,
    ICurrentUser currentUser) : IRequestHandler<CreateBankCommand, int>
{
    private readonly IAggregateRepository<User> _UserRepository = UserRepository;
    private readonly IAggregateRepository<Bank> _bankRepository = bankRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<int> Handle(CreateBankCommand command, CancellationToken cancellationToken)
    {
        var User = await _UserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new UserNotFoundException(_currentUser.UserId);

        var bank = Bank.Create(command.name, command.swiftCode, command.status, command.logo,
                               command.providerId, command.providerData, command.directDebitAmountLimit,
                               command.directDebitDailyTransactionLimit, User.Id);

        await _bankRepository.AddAsync(bank, cancellationToken);

        return bank.Id;
    }
}
