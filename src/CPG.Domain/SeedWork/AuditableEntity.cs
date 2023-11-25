using System;

namespace CPG.Domain.SeedWork;

public abstract class AuditableEntity<T> : Entity<T>
{
    public DateTime CreationDate { get; set; }

    public long CreationUserId { get; set; }

    public DateTime? ModificationDate { get; set; }

    public long? ModificationUserId { get; set; }

    public bool IsActive { get; set; }
}
