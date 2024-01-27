using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.BankAggregate.Exceptions;

namespace CPG.Domain.AggregateModels.BankAggregate;

public class BankDirectDebitSetting : AuditableEntity<long>
{
    public long ProviderId { get; set; }
    public int BankId { get; set; }
    public string DDBankCode { get; set; }
    public decimal MaxWithdrawalAmountPerDay { get; set; }
    public ValidityDuration MaxMandateValidityDurationPerMonth { get; set; }
    public AuthenticationType AuthenticationType { get; set; }    
    public Bank Bank { get; set; }

    public BankDirectDebitSetting()
    {
        
    }

    public BankDirectDebitSetting(long providerId, string ddBankCode, decimal maxWithdrawalAmountPerDay,
        ValidityDuration maxMandateValidityDurationPerMonth, AuthenticationType authenticationType)
    {
        ProviderId = providerId;
        DDBankCode = ddBankCode;
        MaxWithdrawalAmountPerDay = maxWithdrawalAmountPerDay;
        MaxMandateValidityDurationPerMonth = maxMandateValidityDurationPerMonth;
        AuthenticationType = authenticationType;
        IsActive = true;
    }

    public static BankDirectDebitSetting Create(long providerId, string ddBankCode, decimal maxWithdrawalAmountPerDay,
        ValidityDuration maxMandateValidityDurationPerMonth, AuthenticationType authenticationType)
    {
        if (maxWithdrawalAmountPerDay < 1000000 || maxWithdrawalAmountPerDay > 10000000000)
            throw new InvalidMaxWithdrawalAmountPerDayException(maxWithdrawalAmountPerDay);

        if (ddBankCode.Length < 3 || ddBankCode.Length > 255)
            throw new InvalidIvCharachterException(ddBankCode);

        var setting = new BankDirectDebitSetting(providerId, ddBankCode, maxWithdrawalAmountPerDay, 
            maxMandateValidityDurationPerMonth, authenticationType);
        return setting;
    }

    public void Update(long providerId, string ddBankCode, decimal maxWithdrawalAmountPerDay,
        ValidityDuration maxMandateValidityDurationPerMonth, AuthenticationType authenticationType)
    {
        if (maxWithdrawalAmountPerDay < 1000000 || maxWithdrawalAmountPerDay > 10000000000)
            throw new InvalidMaxWithdrawalAmountPerDayException(maxWithdrawalAmountPerDay);

        if (ddBankCode.Length < 3 || ddBankCode.Length > 255)
            throw new InvalidIvCharachterException(ddBankCode);

        ProviderId = providerId;        
        DDBankCode = ddBankCode;
        MaxWithdrawalAmountPerDay = maxWithdrawalAmountPerDay;
        MaxMandateValidityDurationPerMonth = maxMandateValidityDurationPerMonth;
        AuthenticationType = authenticationType;
        IsActive = true;
    }
}