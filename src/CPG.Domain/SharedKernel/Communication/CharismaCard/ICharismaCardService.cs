using CPG.Domain.SharedKernel.Communication.CharismaCard.Models;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.CharismaCard;

public interface ICharismaCardService
{
    Task<Result<CharismaCardUserDepositBalanceResponse>> GetUserDepositBalance(string nationalCode);
    Task<Result<DirectDebitResponse>> DirectDebitRequest(DirectDebitRequest request);
}