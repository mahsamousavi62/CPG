using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

namespace CCPG.Domain.SharedKernel.Communication.Ipg;
public interface IIpgProvider
    {
        Task<PaymentTicketResponse> GetPaymentTicketAsync(PaymentTicketRequest paymentIpgRequest);
    }   

