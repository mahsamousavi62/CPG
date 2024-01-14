using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;
using MediatR;

namespace CPG.Application.UseCases.CharisPayServices.Queries;

public class GetAccountNumberQuery(string iban) : IRequest<Result<AccountNumberResponse>>
{
    public string Iban { get; set; } = iban;
}
