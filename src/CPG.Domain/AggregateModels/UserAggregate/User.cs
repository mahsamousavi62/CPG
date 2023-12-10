using CPG.Domain.SeedWork;
using System;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Threading.Tasks;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public class User : Entity<long>, IAggregateRoot
    {
        public User()
        {
            
        }
        public User(string iDPId, string nationalCode, string firstName, string lastName, string phoneNumber)
        {
            IDPId = iDPId;
            NationalCode = nationalCode;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            IsLegal = true;
            KYCStatus = 1;
            LastUpdateFromIDP = DateTime.Now;
            CreatationDateTime = DateTime.Now;
        }

        public User(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = 1;
            LastUpdateFromIDP = DateTime.Now;
            CreatationDateTime = DateTime.Now;
        }

        #region [ Fields And Properties ]

        public string IDPId { get; }

        public string NationalCode { get; }

        public long? CompanyId { get; set; }

        public string FirstName { get; }

        public string LastName { get; }

        public string PhoneNumber { get; }

        public DateTime LastUpdateFromIDP { get; set; }

        public short KYCStatus { get; }

        public bool IsLegal { get; }

        public bool IsActive { get; }

        public DateTime CreatationDateTime { get; set; }

        public DateTime ModificationDate { get; set; }

        public List<UserRole> UserRoles { get; set; } = [];

        public Company Company { get; set; }

        #endregion

        public static User Create(string iDPId, NationalCode nationalCode, Name name, PhoneNumber phoneNumber, short userRoleType)
        {
            var user = new User(iDPId, nationalCode.Value, name.FirstName, name.LastName, phoneNumber.Value);
            user.UserRoles.Add(new UserRole(userRoleType));
           
            return user;
        }

        public static User Update(Name name, string phoneNumber)
        {
            var user = new User(name.FirstName, name.LastName, phoneNumber);

            return user;
        }

        public void GetIdpUserProfile(GetIdpUserProfileModel model)
        {
            AddDomainEvent(new GetIdpUserProfileEvent(model));
        }

        public static void UpdateUserCompany(List<User> users, long companyId) => users.ForEach(user => { user.CompanyId = companyId; });

        public static  Task<string> GetUserName(long userId)
        {
            return Task.FromResult("CPG Test");
        }
    }
}
