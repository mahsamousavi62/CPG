using CPG.Application.UseCases.CharisPayServices.Queries;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CharisPayServices;

public class GetAccountNumberQueryHandler : IRequestHandler<GetAccountNumberQuery, Result<AccountNumberResponse>>
{
    private readonly ICharisPayProvider _charisPayClient;
    public GetAccountNumberQueryHandler(ICharisPayProvider charisPayClient)
    {
        _charisPayClient = charisPayClient;
    }

    public async Task<Result<AccountNumberResponse>> Handle(GetAccountNumberQuery request, CancellationToken cancellationToken)
    {
        var iban = new Iban(request.Iban);
        var accountNumber = await _charisPayClient.GetAccountNumber(iban.Value);

        return Result<AccountNumberResponse>.SuccessResult(accountNumber.Data);
    }
}
