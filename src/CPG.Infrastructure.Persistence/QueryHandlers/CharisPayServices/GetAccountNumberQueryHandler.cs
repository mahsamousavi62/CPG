
using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ClientFactory;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CharisPayServices
{
    public class GetAccountNumberQueryHandler : IRequestHandler<GetAccountNumberQuery, ResultData<AccountNumberViewModel>>
    {
        private readonly ICharisPayClient _charisPayClient;
        public GetAccountNumberQueryHandler(ICharisPayClient charisPayClient)
        {
            _charisPayClient = charisPayClient;
        }
        public async Task<ResultData<AccountNumberViewModel>> Handle(GetAccountNumberQuery request, CancellationToken cancellationToken)
        {
            var iban = new Iban(request.Iban);
            var accountNumber = await _charisPayClient.GetAccountNumber(iban.Value);
            return accountNumber;
        }
    }
}
