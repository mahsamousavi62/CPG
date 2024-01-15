using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class CancelPaymentRequestViewModel
{
    [Required]
    public string PaymentCode { get; set; }
}
