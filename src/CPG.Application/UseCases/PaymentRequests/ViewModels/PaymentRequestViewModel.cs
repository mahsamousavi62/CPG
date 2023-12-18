using System;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels
{
    public class PaymentRequestResponseViewModel
    {
        public string Code { get; set; }
        public string PageUrl { get; set; }
        public short Status { get; set; }
        public DateTime ExpirationDateTime { get; set; }
    }
}
