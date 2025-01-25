using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels
{
    public class AnonymousStatusViewModel
    {
        [Required]
        public string PaymentCode { get; set; }
    }
}
