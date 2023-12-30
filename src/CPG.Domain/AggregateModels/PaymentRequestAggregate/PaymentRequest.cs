using System;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

public class PaymentRequest : AuditableEntity<long>, IAggregateRoot
{
    public PaymentRequest()
    {

    }
    public PaymentRequest(long companyId, string destinationIban, long applicationId, string nationalCode,
    string description, decimal amount, string callBackUrl, string code, string trackerId, short status, bool isUsed)
    {
        CompanyId = companyId;
        DestinationIban = destinationIban;
        ApplicationId = applicationId;
        NationalCode = nationalCode;
        Description = description;
        Amount = amount;
        CallBackUrl = callBackUrl;
        Code = code;
        TrackerId = trackerId;
        Status = status;
        IsUsed = isUsed;
    }


    public long CompanyId { get; set; }
    public long ApplicationId { get; set; }
    public string DestinationIban { get; set; }
    public string NationalCode { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string CallBackUrl { get; set; }
    public string Code { get; set; }
    public string TrackerId { get; set; }
    public short Status { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? VerificationDateTime { get; set; }
    public DateTime UrlExpirationDateTime { get; set; }
    public Application Application { get; set; }
    public Company Company { get; set; }
    public Transaction Transaction { get; set; }
    public static PaymentRequest Create(PaymentRequest paymentRequest, int expireTime, string clientId, string applicationEnglishName)
    {
        paymentRequest.UrlExpirationDateTime = DateTime.UtcNow.AddMinutes(expireTime);
        paymentRequest.IsActive = true;
        string hexString = Guid.NewGuid().ToString("N");
        string randomString = hexString.Substring(0, 16);
        paymentRequest.Code = $"{applicationEnglishName}_{clientId}_{randomString}";
        paymentRequest.Status = 0;
        paymentRequest.IsActive = true;
        paymentRequest.IsUsed = false;
        return paymentRequest;
    }

    public static void Update(PaymentRequest paymentRequest)
    {
        paymentRequest.IsUsed = true;
        paymentRequest.Status = (short)Enums.PaymentStatus.InProgress;
        paymentRequest.ModificationDate = DateTime.UtcNow;
    }

    public static void UpdateStatus(PaymentRequest paymentRequest, Enums.PaymentStatus status)
    {
        paymentRequest.Status = (short)status;
    }
}