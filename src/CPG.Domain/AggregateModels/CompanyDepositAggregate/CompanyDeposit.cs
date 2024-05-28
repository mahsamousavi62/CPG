using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Events;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CPG.Domain.AggregateModels.CompanyDepositAggregate;

public class CompanyDeposit : AuditableEntity<long>, IAggregateRoot
{
    public string Name { get; set; }
    public int BankId { get; set; }
    public string Iban { get; set; }
    public string AccountNumber { get; set; }
    public long CompanyId { get; set; }
    public bool? IsDefaultForDirectDebit { get; set; }
    public bool? IsDefaultForCharismaCard { get; set; }

    public Company Company { get; set; }
    public Bank Bank { get; set; }
    public List<CompanyIPGDeposit> CompanyIPGDeposits { get; set; }
    public List<Transaction> Transactions { get; set; }
    public List<CompanyDepositPaymentMethod> PaymentMethods { get; set; } = [];
    public List<PaymentRequestMethodDeposit> PaymentRequestMethodDeposits { get; set; }

    public CompanyDeposit()
    {

    }

    public CompanyDeposit(PersianName name, Iban iban, int bankId, string accountNumber, long companyId, bool isDefaultForDirectDebit)
    {
        Guard.Against.NullOrEmpty(accountNumber);

        Name = name.Value;
        Iban = iban.Value;
        BankId = bankId;
        AccountNumber = accountNumber;
        CompanyId = companyId;
        IsDefaultForDirectDebit = isDefaultForDirectDebit;
        IsActive = true;
    }

    public static CompanyDeposit Create(PersianName name, Iban iban, int bankId, string accountNumber, long companyId, bool isDefaultForDD,
        Enums.PaymentMethodType[] details)
    {
        var companyDeposit = new CompanyDeposit(name, iban, bankId, accountNumber, companyId, isDefaultForDD);

        var paymentMethods = CompanyDepositPaymentMethod.Create(details);
        companyDeposit.PaymentMethods.AddRange(paymentMethods);

        companyDeposit.AddDomainEvent(new NewCompanyDepositCreatedEvent(companyDeposit.Id, DateTime.Now));

        return companyDeposit;
    }

    public static void Update(CompanyDeposit companyDeposit, PersianName persianName, Enums.PaymentMethodType[] details)
    {
        companyDeposit.Name = persianName.Value;

        foreach (var newItem in details)
        {
            if (!companyDeposit.PaymentMethods.Any(p => p.MethodType == newItem))
                companyDeposit.PaymentMethods.Add(CompanyDepositPaymentMethod.Create(newItem));
        }

        foreach (var currnetItem in companyDeposit.PaymentMethods.ToList())
        {
            if (!details.Any(p => p == currnetItem.MethodType))
                companyDeposit.PaymentMethods.Remove(currnetItem);
        }
    }
}