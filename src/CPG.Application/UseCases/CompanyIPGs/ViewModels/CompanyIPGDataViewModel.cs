using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class CompanyIPGDataViewModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ProviderId { get; set; }
    public long IPGTypeId { get; set; }
    public string ProviderData { get; set; }
    public string IPGTypeLogo { get; set; }
    public string IPGTypeName { get; set; }
    public string ProviderName { get; set; }
    public List<CompanyIPGDepositDataViewModel> Deposits { get; set; }
    public CompanyIPGDepositDataViewModel DefaultDeposit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
}
