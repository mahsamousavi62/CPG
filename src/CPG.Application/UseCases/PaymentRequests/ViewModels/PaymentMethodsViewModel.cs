using CPG.Application.UseCases.DirectDebit.ViewModels;
using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class PaymentMethodsViewModel
{
    public decimal Amount { get; set; }
    public string PaymentCode { get; set; }
    public List<IPGInfo> IPGs { get; set; }
    public List<DirectDebitInfo> DirectDebits { get; set; }
    public Receipt Receipt { get; set; }
    public string CompanyName { get; set; }
}

public class IPGInfo
{
    public long Id { get; set; }
    
    public string PersianName { get; set; }

    public string Logo { get; set; }
}

public class DirectDebitInfo
{
    public AvailableBankViewModel BankInfo { get; set; }

    public List<DirectDebitGrantInfo> GrantInfo { get; set; }
}

public class DirectDebitGrantInfo
{
    public long Id { get; set; }
    public string AccountNumber { get; set; }
}

public class Receipt
{
    public long? DestinationDepositId { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
}

public class CharismaCard
{
    public long BalanceAmount { get; set; }
    public string CustomerSurname { get; set; }
    public string VardNumber { get; set; }
    public DateTime ExpirationDate { get; set; }
}
