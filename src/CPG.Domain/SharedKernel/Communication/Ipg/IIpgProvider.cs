using System.Threading.Tasks;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.TransactionResult;

namespace CCPG.Domain.SharedKernel.Communication.Ipg;
public interface IIpgProvider
{
    Task<ResultData<PaymentTicketResponse>> GetPaymentTicketAsync(PaymentTicketRequest paymentIpgRequest);

    Task<ResultData<TransactionResultResponse>> GetTransactionResult(TransactionResultRequest transactionResultRequest);
}

