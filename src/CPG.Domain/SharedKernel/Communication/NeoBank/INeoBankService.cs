using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.NeoBank.Models;

namespace CPG.Domain.SharedKernel.Communication.NeoBank;

public interface INeoBankService
{
    Task<Result<UserDepositBalanceResponse>> GetUserDepositBalance();
    Task<Result<ClientDirectDebitResponse>> ClientDirectDebit();
}
