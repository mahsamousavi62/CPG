using Ardalis.GuardClauses;
using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Events;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyDeposits;

public class CompanyDeposit : AuditableEntity<long>, IAggregateRoot
{
    public string Name { get; set; }
    public int BankId { get; set; }
    public string Iban { get; set; }
    public string AccountNumber { get; set; }
    public long CompanyId { get; set; }
    public bool? IsDefaultForDD { get; set; }

    public Company Company { get; set; }
    public Bank Bank { get; set; }
    public List<CompanyIPGDeposit> CompanyIPGDeposits { get; set; }
    public List<Transaction> Transactions { get; set; }

    public CompanyDeposit()
    {

    }

    public CompanyDeposit(PersianName name, Iban iban, int bankId, string accountNumber, long companyId, bool isDefaultForDD)
    {
        Guard.Against.NullOrEmpty(accountNumber);

        Name = name.Value;
        Iban = iban.Value;
        BankId = bankId;
        AccountNumber = accountNumber;
        CompanyId = companyId;
        IsDefaultForDD = isDefaultForDD;
        IsActive = true;
    }

    public static CompanyDeposit Create(PersianName name, Iban iban, int bankId, string accountNumber, long companyId, bool isDefaultForDD)
    {
        var companyDeposit = new CompanyDeposit(name, iban, bankId, accountNumber, companyId, isDefaultForDD);

        companyDeposit.AddDomainEvent(new NewCompanyDepositCreatedEvent(companyDeposit.Id, DateTime.Now));

        return companyDeposit;
    }

    public static void Update(CompanyDeposit companyDeposit, PersianName persianName)
    {
       companyDeposit.Name=persianName.Value;
    }
}
