using CPG.Domain.AggregateModels.TransactionAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyIPGReadModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ProviderId { get; set; }
    public long IPGTypeId { get; set; }
    public string ProviderData { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public CompanyReadModel Company { get; set; }
    public ProviderReadModel Provider { get; set; }
    public IPGTypeReadModel IPGType { get; set; }
    public ICollection<CompanyIPGDepositReadModel> CompanyIPGDeposits { get; set; }
    public ICollection<IPGTransactionReadModel> IPGTransactions { get; set; }

}
