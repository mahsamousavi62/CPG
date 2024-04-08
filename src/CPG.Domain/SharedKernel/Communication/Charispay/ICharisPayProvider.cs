using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Charispay.Models.AccountNumber;

namespace CPG.Domain.SharedKernel.Communication.Charispay;

public interface ICharisPayProvider
{
    Task<Result<AccountNumberResponse>> GetAccountNumber(string iban);
}

