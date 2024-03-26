using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using System.Linq;

namespace CPG.Domain.AggregateModels.UserAggregate
{
    public class User : AuditableEntity<long>, IAggregateRoot
    {
        public User()
        {

        }
        public User(string idpId, string nationalCode, string firstName, string lastName, string phoneNumber, bool isLegal, short kycStatus)
        {
            IdpId = idpId;
            NationalCode = nationalCode;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = kycStatus;
            IsActive = true;
            IsLegal = isLegal;
        }

        public User(string firstName, string lastName, string phoneNumber, bool isLegal, short kycStatus)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = kycStatus;
            IsLegal = isLegal;
        }

        #region [ Fields And Properties ]

        public string IdpId { get; private set; }

        public string NationalCode { get; private set; }

        public long? CompanyId { get;  set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string PhoneNumber { get; private set; }

        public DateTime? LastUpdateFromIDP { get; private set; }

        public short? KYCStatus { get; private set; }

        public bool IsLegal { get; private set; }

        public List<UserRole> UserRoles { get; set; } = [];

        public Company Company { get; set; }

        #endregion

        public static User Create(string idpId, NationalCode nationalCode, Name name,
            PhoneNumber phoneNumber, UserRoleType userRoleType, bool isLegal, short kycStatus)
        {
            var user = new User(idpId, nationalCode.Value, name.FirstName, name.LastName, phoneNumber.Value, isLegal, kycStatus);
            user.UserRoles.Add(new UserRole(userRoleType));
            user.LastUpdateFromIDP = DateTime.Now;

            return user;
        }

        public static User Update(User user, Name name, string phoneNumber, string sub, bool isLegal, short kycStatus)
        {
            user.FirstName = name.FirstName;
            user.LastName = name.LastName;
            user.PhoneNumber = phoneNumber;
            user.KYCStatus = kycStatus;
            user.LastUpdateFromIDP = DateTime.Now;
            user.IdpId = sub;
            user.IsLegal = isLegal;

            return user;
        }

        public static void UpdateUserCompany( List<User> users, long companyId, Company company=null)
        {
            if(company!=null)
            foreach (var currnetItem in company.Users.ToList())
            {
                currnetItem.CompanyId = null;
                var userRole = currnetItem.UserRoles.Find(ur => ur.RoleType == UserRoleType.CompanyUser);
                currnetItem.UserRoles.Remove(userRole);
            }
            users.ForEach(user =>
            {
                user.CompanyId = companyId;
                if (!user.UserRoles.Any(item => item.RoleType == UserRoleType.CompanyUser))
                    user.UserRoles.Add(new UserRole(UserRoleType.CompanyUser));
            });
        }

        public static Task<string> GetUserName(long userId)
        {
            return Task.FromResult("CPG Test");
        }
    }
}