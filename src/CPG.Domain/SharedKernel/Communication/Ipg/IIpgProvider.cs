using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;

namespace CCPG.Domain.SharedKernel.Communication.Ipg;
public interface IIpgProvider
{
    Task<PaymentTokenResponse> GetPaymentTokenAsync(PaymentTokenRequest paymentIpgRequest);

    Task<TransactionResultResponse> GetTransactionResult(TransactionResultRequest transactionResultRequest);
}

