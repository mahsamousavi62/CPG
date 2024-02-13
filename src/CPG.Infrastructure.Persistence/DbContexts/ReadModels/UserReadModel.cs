using System;
using System.Collections.Generic;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class UserReadModel
{
    public long Id { get; set; }
    public string IDPId { get; set; }
    public string NationalCode { get; set; }
    public long? CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime? LastUpdateFromIDP { get; set; }
    public short? KYCStatus { get; }
    public bool IsLegal { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public List<UserRoleReadModel> UserRoles { get; set; }
    public CompanyReadModel Company { get; set; }
}
