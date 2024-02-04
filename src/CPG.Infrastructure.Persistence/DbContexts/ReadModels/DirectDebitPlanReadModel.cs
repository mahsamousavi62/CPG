using System;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class DirectDebitPlanReadModel
{
    public int Id { get; set; }
    public short DurationPerMonth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
}
