using System;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels
{
    public class PaymentRequestViewModel
    {
        public long Id { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string DestinationIban { get; set; }
        public string NationalCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string CallBackUrl { get; set; }
        public string Code { get; set; }
        public string TrackerId { get; set; }
        public bool IsVerified { get; set; }
        public short Status { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? VerificationDateTime { get; set; }
        public DateTime UrlExpirationDateTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
