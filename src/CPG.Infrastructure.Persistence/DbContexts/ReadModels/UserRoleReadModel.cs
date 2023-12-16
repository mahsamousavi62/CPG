using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class UserRoleReadModel
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public UserRoleType RoleType { get; set; }
    public bool IsActive { get; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public UserReadModel User { get; set; }
}
