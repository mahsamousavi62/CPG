using System;

namespace CPG.Domain.AggregateModels.UserAggregate;

public class UserRole(short roleType)
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public short RoleType { get; set; } = roleType;
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public DateTime ModificationDate { get; set; }

}
