using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.Communication.Ipg;
using CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket;

namespace CPG.Infrastructure.Providers.Ipg
{
    public class AsanPardakhtProvider : IIpgProvider
    {
        public Task<PaymentTicketResponse> GetPaymentTicketAsync(PaymentTicketRequest paymentIpgRequest)
        {
            throw new NotImplementedException();
        }
    }
}
