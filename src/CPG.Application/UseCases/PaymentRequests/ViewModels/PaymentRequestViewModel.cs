using CPG.Domain.SharedKernel;
using System;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels
{
    public class PaymentRequestResponseViewModel
    {
        public string PaymentCode { get; set; }
        public string PageUrl { get; set; }
        public Enums.PaymentStatus Status { get; set; }
        public string ExpirationDateTime { get; set; }
    }
}
