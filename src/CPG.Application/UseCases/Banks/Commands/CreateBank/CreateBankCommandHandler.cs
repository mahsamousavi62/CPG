using CPG.Application.UseCases.Books.Commands.AddBook;
using CPG.Application.UseCases.CPGUsers.Exceptions;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.BookAggregate;
using CPG.Domain.AggregateModels.CPGUserAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Banks.Commands.CreateBank
{
    public class CreateBankCommandHandler : IRequestHandler<CreateBankCommand, int>
    {
        private readonly IAggregateRepository<CPGUser> _CPGUserRepository;
        private readonly IAggregateRepository<Bank> _bankRepository;
        private readonly ICurrentUser _currentUser;

        public CreateBankCommandHandler(
            IAggregateRepository<CPGUser> CPGUserRepository,
            IAggregateRepository<Bank> bankRepository,
            ICurrentUser currentUser)
        {
            _CPGUserRepository = CPGUserRepository;
            _bankRepository = bankRepository;
            _currentUser = currentUser;
        }

        public async Task<int> Handle(CreateBankCommand command, CancellationToken cancellationToken)
        {
            var cpgUser = await _CPGUserRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
                ?? throw new CPGUserNotFoundException(_currentUser.UserId);

            var bank = Bank.Create(command.name, command.swiftCode, command.status, command.logo,
                                   command.providerId, command.providerData, command.directDebitAmountLimit,
                                   command.directDebitDailyTransactionLimit, cpgUser.Id);

            await _bankRepository.AddAsync(bank, cancellationToken);

            return bank.Id;
        }
    }
}
