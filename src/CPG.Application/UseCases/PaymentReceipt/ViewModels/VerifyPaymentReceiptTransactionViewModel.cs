

using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentReceipt.ViewModels
{
    public class VerifyPaymentReceiptTransactionViewModel
    {
        [Required]
        public long Id { get; set; }
    }
}
