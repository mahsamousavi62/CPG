using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class PaymentMethodsViewModel
{
    public decimal Amount { get; set; }
    public List<IPGInfo> IPGs { get; set; }
}

public class IPGInfo
{
    public long Id { get; set; }
    
    public string PersianName { get; set; }

    public string Logo { get; set; }
}

