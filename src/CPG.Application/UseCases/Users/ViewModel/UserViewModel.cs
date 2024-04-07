using System.Collections.Generic;
using CPG.Domain.AggregateModels.UserAggregate;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Users.ViewModel;

public class UserViewModel
{
    public long Id { get; set; }
    public string IDPId { get; set; }
    public string NationalCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string CompanyLogo { get; set; }
    public string CompanyPersianName { get; set; }
    public Dictionary<byte,string> UserRoles { get; set; }
    public byte[] UserRolesArray { get; set; }
    public long? CompanyId { get; set; }
}
