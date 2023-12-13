using CPG.Domain.SeedWork;
using System;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public class User : AuditableEntity<long>, IAggregateRoot
    {
        public User()
        {

        }
        public User(string idpId, string nationalCode, string firstName, string lastName, string phoneNumber, long companyId)
        {
            IdpId = idpId;
            NationalCode = nationalCode;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = 1;
            CompanyId = companyId;  
            IsActive = true;
            IsLegal = false;
        }

        public User(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = 1;
        }

        #region [ Fields And Properties ]

        public string IdpId { get; }

        public string NationalCode { get; }

        public long? CompanyId { get; set; }

        public string FirstName { get; }

        public string LastName { get; }

        public string PhoneNumber { get; }

        public DateTime? LastUpdateFromIDP { get; set; }

        public short? KYCStatus { get; }

        public bool IsLegal { get; }

        public List<UserRole> UserRoles { get; set; } = [];

        public Company Company { get; set; }

        #endregion

        public static User Create(string idpId, NationalCode nationalCode, Name name, PhoneNumber phoneNumber, long companyId, UserRoleType userRoleType)
        {
            var user = new User(idpId, nationalCode.Value, name.FirstName, name.LastName, phoneNumber.Value, companyId);
            user.UserRoles.Add(new UserRole(userRoleType));
            user.LastUpdateFromIDP = DateTime.UtcNow;            
            return user;
        }

        public static User Update(Name name, string phoneNumber)
        {
            var user = new User(name.FirstName, name.LastName, phoneNumber);
            user.LastUpdateFromIDP = DateTime.UtcNow;
            return user;
        }

        public void GetIdpUserProfile(GetIdpUserProfileModel model)
        {
            AddDomainEvent(new GetIdpUserProfileEvent(model));
        }

        public static void UpdateUserCompany(List<User> users, long companyId) => users.ForEach(user => { user.CompanyId = companyId; });

        public static Task<string> GetUserName(long userId)
        {
            return Task.FromResult("CPG Test");
        }
    }
}
