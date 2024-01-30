using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit;

public interface IDirectDebitProvider
{
    Task<TokenResponse> GetTokenAsync(TokenRequest paymentIpgRequest);
}
