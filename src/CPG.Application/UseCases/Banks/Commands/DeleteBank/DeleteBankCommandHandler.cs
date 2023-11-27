using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.DeleteBank
{
    public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand>
    {
        private readonly IAggregateRepository<User> _UserRepository;
        private readonly IAggregateRepository<Bank> _bankRepository;
        private readonly ICurrentUser _currentUser;

        public DeleteBankCommandHandler(
            IAggregateRepository<User> UserRepository,
            IAggregateRepository<Bank> bankRepository,
            ICurrentUser currentUser)
        {
            _UserRepository = UserRepository;
            _bankRepository = bankRepository;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            _ = await _UserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new UserNotFoundException(_currentUser.UserId);

            var bank = await _bankRepository.GetByIdAsync(request.bankId, cancellationToken)
                ?? throw new BankNotFoundException(request.bankId);

            await _bankRepository.DeleteAsync(bank, cancellationToken);
        }
    }
}
