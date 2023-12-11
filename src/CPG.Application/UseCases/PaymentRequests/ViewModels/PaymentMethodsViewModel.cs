using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.ViewModels;

public class PaymentMethodsViewModel
{
    public decimal Amount { get; set; }
    public List<CompanyIPG> IPGs { get; set; }
}

