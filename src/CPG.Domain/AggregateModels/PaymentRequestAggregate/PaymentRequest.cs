using System;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.PaymentRequestAggregate.UseCases.PaymentRequests.Exceptions;

public class PaymentRequest : AuditableEntity<long>, IAggregateRoot
{
    public PaymentRequest()
    {

    }

    public PaymentRequest(long companyId, string destinationIban, long applicationId, string nationalCode, string description, decimal amount,
        string callBackUrl, string code, string trackerId, Enums.PaymentStatus status, bool isUsed, string paymentIdentifier)
    {
        CompanyId = companyId;
        DestinationDepositIban = destinationIban;
        ApplicationId = applicationId;
        NationalCode = nationalCode;
        Description = description;
        Amount = amount;
        CallBackUrl = callBackUrl;
        PaymentCode = code;
        TrackerId = trackerId;
        PaymentIdentifier = paymentIdentifier;
        Status = status;
        IsUsed = isUsed;
    }

    public long CompanyId { get; set; }
    public long ApplicationId { get; set; }
    public string DestinationDepositIban { get; set; }
    public string NationalCode { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string CallBackUrl { get; set; }
    public string PaymentCode { get; set; }
    public string TrackerId { get; set; }
    public string PaymentIdentifier { get; set; }
    public Enums.PaymentStatus Status { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime UrlExpirationDateTime { get; set; }
    public Application Application { get; set; }
    public Company Company { get; set; }
    public Transaction Transaction { get; set; }

    public static PaymentRequest Create(PaymentRequest paymentRequest, int expireTime, string clientId, string applicationEnglishName)
    {
        if (!string.IsNullOrEmpty(paymentRequest.PaymentIdentifier) && (paymentRequest.PaymentIdentifier.Length < 5 || paymentRequest.PaymentIdentifier.Length > 255))
            throw new InvalidPaymentIdLengthException();
        paymentRequest.UrlExpirationDateTime = DateTime.Now.AddMinutes(expireTime);
        paymentRequest.IsActive = true;
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 16);
        paymentRequest.PaymentCode = $"{applicationEnglishName}_{clientId}_{randomString}";
        paymentRequest.Status = 0;
        paymentRequest.IsActive = true;
        paymentRequest.IsUsed = false;
        return paymentRequest;
    }

    public static void Update(PaymentRequest paymentRequest)
    {
        paymentRequest.ModificationDate = DateTime.Now;
    }

    public static void UpdateStatus(PaymentRequest paymentRequest, Enums.PaymentStatus status)
    {
        paymentRequest.Status = status;
    }

    public static void Validate(PaymentRequest paymentRequest)
    {
        if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();

        if (!paymentRequest.Company.IsActive) throw new PaymentRequestInActiveCompanyException();

        if (paymentRequest.UrlExpirationDateTime < DateTime.Now) throw new PaymentRequestCodeExpiredException();

        if (paymentRequest.IsUsed) throw new PaymentRequestCodeIsUsedBeforeException();
                                                                                     
        if (paymentRequest.Status != Enums.PaymentStatus.RedirectedToCpg) throw new PaymentRequestCodeInvalidStatusException();
    }
}