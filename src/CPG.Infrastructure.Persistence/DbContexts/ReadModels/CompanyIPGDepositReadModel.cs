using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyIPGDepositReadModel
{
    public long Id { get; set; }
    public long CompanyIPGId { get; set; }
    public long CompanyDepositId { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }

    [ForeignKey("CompanyIPGId")]
    public CompanyIPGReadModel CompanyIPG { get; set; }

    [ForeignKey("CompanyDepositId")]
    public CompanyDepositReadModel CompanyDeposit { get; set; }
}
