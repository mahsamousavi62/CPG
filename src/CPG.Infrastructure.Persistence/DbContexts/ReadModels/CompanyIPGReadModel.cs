using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.IPGTypeAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate;
using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyIPGReadModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ProviderId { get; set; }
    public long IPGTypeId { get; set; }
    public string ProviderData { get; set; }
    public short VerificationTimeLimit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public Company Company { get; set; }
    public Provider Provider { get; set; }
    public IPGType IPGType { get; set; }
    public ICollection<CompanyIPGDepositReadModel> CompanyIPGDeposits { get; set; }
}
