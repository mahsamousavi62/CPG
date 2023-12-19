using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class GetPaymentMethodsViewModel
{
    [Required]
    public string Code { get; set; }
}
