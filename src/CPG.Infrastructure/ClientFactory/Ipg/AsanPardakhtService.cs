using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCPG.Domain.SharedKernel.ClientFactory.Ipg;
using CPG.Domain.SharedKernel.ClientFactory.Ipg;

namespace CPG.Infrastructure.ClientFactory.Ipg
{
    public class AsanPardakhtService : IIpgService
    {
        public Task<PaymentIpgResponse> GetPaymentTicketAsync(PaymentIpgRequest paymentIpgRequest)
        {
            throw new NotImplementedException();
        }
    }
}
