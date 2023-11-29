
using Api.Juros.Infrastructure.External;
using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.AggregateModels.BankAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CharisPayServices
{
    public class GetAccountNumberQueryHandler : IRequestHandler<GetAccountNumberQuery, AccountNumberViewModel>
    {
        private readonly ICharisPayClient _charisPayClient;
        public GetAccountNumberQueryHandler(ICharisPayClient charisPayClient)
        {
            _charisPayClient = charisPayClient;
        }
        public async Task<AccountNumberViewModel> Handle(GetAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var iban = new Iban(request.Iban);
            var accountNumber = await _charisPayClient.GetAccountNumber(iban.Value);
            return accountNumber;
        }
    }
}
