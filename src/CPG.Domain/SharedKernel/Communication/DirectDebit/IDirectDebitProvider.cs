using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Show;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Store;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Token;
using CPG.Domain.SharedKernel.Communication.DirectDebit.Models.Verify;
using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.Communication.DirectDebit;

public interface IDirectDebitProvider
{
    Task<TokenResponse> GetTokenAsync(TokenRequest request);

    Task<StoreResponse> StoreAsync(StoreRequest request);

    Task<ShowResponse> ShowAsync(ShowRequest request);

    Task<VerifyResponse> VerifyAsync(VerifyRequest request);

    Task<UserGrantsResponse> GetUserGrants(UserGrantsRequest request);

    Task<WithdrawalResponse> WithdrawAsync(WithdrawalRequest request);
}
