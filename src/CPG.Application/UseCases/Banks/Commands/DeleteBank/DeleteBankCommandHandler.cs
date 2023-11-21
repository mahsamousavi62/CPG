using CPG.Application.UseCases.Banks.Commands.CreateBank;
using CPG.Application.UseCases.Banks.Exceptions;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.DeleteBank
{
    public class DeleteBankCommandHandler : IRequestHandler<DeleteBankCommand>
    {
        private readonly IAggregateRepository<CPGUser> _CPGUserRepository;
        private readonly IAggregateRepository<Bank> _bankRepository;
        private readonly ICurrentUser _currentUser;

        public DeleteBankCommandHandler(
            IAggregateRepository<CPGUser> CPGUserRepository,
            IAggregateRepository<Bank> bankRepository,
            ICurrentUser currentUser)
        {
            _CPGUserRepository = CPGUserRepository;
            _bankRepository = bankRepository;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteBankCommand request, CancellationToken cancellationToken)
        {
            _ = await _CPGUserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new CPGUserNotFoundException(_currentUser.UserId);

            var bank = await _bankRepository.GetByIdAsync(request.bankId, cancellationToken)
                ?? throw new BankNotFoundException(request.bankId);

            await _bankRepository.DeleteAsync(bank, cancellationToken);
        }
    }
}
