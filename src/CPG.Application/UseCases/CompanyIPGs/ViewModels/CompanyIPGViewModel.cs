using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using System;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CompanyIPGs.ViewModels;

public class CompanyIPGViewModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ProviderId { get; set; }
    public long IPGTypeId { get; set; }
    public string ProviderData { get; set; }
    public Company Company { get; set; }
    public Provider Provider { get; set; }
    public IPGType IPGType { get; set; }
    public List<CompanyIPGDepositViewModel> IPGDeposits { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
}
