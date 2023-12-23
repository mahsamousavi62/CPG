using System;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket
{
    public class PaymentTicketRequest
    {
        public long CompanyIPGId { get; set; }
        public long PaymentRequestId { get; set; }
    }
}
