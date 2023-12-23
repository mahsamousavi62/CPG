using System;

namespace CPG.Domain.SharedKernel.Communication.Ipg.Models.PaymentTicket
{
    public class Params
    {
        public string Token { get; set; }
    }

    public class PaymentTicketResponse
    {
        public string Url { get; set; }
        public string Method => "Post";
        public Params Params { get; set; }
    }
}
