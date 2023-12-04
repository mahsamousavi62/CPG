using System.ComponentModel.DataAnnotations;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels
{
    public class CreatePaymentRequestViewModel
    {
        public long? CompanyId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string CallBackUrl { get; set; }
        public string DestinationIban { get; set; }
        [Required]
        public string NationalCode { get; set; }
        [Required]
        public string TrackerId { get; set; }
        public string Description { get; set; }
    }
}
