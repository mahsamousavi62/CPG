using CPG.Domain.SeedWork;
using System;
using System.Collections.Generic;
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
        public User(string idpId, string nationalCode, string firstName, string lastName, string phoneNumber)
        {
            IdpId = idpId;
            NationalCode = nationalCode;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            KYCStatus = 1;
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

        public string IdpId { get; private set; }

        public string NationalCode { get; private set; }

        public long? CompanyId { get; private set; }

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
            PhoneNumber phoneNumber, UserRoleType userRoleType)
        {
            var user = new User(idpId, nationalCode.Value, name.FirstName, name.LastName, phoneNumber.Value);
            user.UserRoles.Add(new UserRole(userRoleType));
            user.LastUpdateFromIDP = DateTime.Now;

            return user;
        }

        public static User Update(User user, Name name, string phoneNumber, string sub)
        {
            user.FirstName = name.FirstName;
            user.LastName = name.LastName;
            user.PhoneNumber = phoneNumber;
            user.KYCStatus = 1;
            user.LastUpdateFromIDP = DateTime.Now;
            user.IdpId = sub;

            return user;
        }

        public static void UpdateUserCompany(List<User> users, long companyId) => users.ForEach(user => { user.CompanyId = companyId; });

        public static Task<string> GetUserName(long userId)
        {
            return Task.FromResult("CPG Test");
        }


    }
}
