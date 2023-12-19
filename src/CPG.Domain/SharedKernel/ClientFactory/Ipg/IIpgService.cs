using System.Threading.Tasks;
using CPG.Domain.SharedKernel.ClientFactory.Ipg;

namespace CCPG.Domain.SharedKernel.ClientFactory.Ipg;
    public interface IIpgService
    {
        Task<PaymentIpgResponse> GetPaymentTicketAsync(PaymentIpgRequest paymentIpgRequest);
    }   

