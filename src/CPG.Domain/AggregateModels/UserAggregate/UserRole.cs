using CPG.Domain.SeedWork;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.UserAggregate;

public class UserRole : AuditableEntity<long>
{
    public UserRole(UserRoleType roleType)
    {
        RoleType = roleType;
        CreationDate = DateTime.Now;
    }

    public long UserId { get; set; }
    public UserRoleType RoleType { get; set; }
    public User User { get; set; }
}
