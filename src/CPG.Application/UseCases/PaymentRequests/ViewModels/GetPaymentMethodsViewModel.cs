using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class GetPaymentMethodsViewModel
{
    [Required]
    public string PaymentCode { get; set; }
}
