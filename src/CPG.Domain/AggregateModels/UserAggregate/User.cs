using CPG.Domain.SeedWork;
using System;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using System.Collections.Generic;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;

namespace CPG.Domain.AggregateModels.UserAggregate;

public class User : Entity<long>, IAggregateRoot
{
    public User(string iDPId, string nationalCode, string firstName, string lastName, string phoneNumber)
    {
        _iDPId = iDPId;
        _nationalCode = nationalCode;
        _firstName = firstName;
        _lastName = lastName;
        _phoneNumber = phoneNumber;
        _isActive = true;
        _kYCStatus = 1;
        LastUpdateFromIDP = DateTime.Now;
        CreatationDateTime = DateTime.Now;
        ModificationDate = DateTime.Now;
    }

    public User(string firstName, string lastName, string phoneNumber)
    {
        _firstName = firstName;
        _lastName = lastName;
        _phoneNumber = phoneNumber;
        _kYCStatus = 1;
        LastUpdateFromIDP = DateTime.Now;
        CreatationDateTime = DateTime.Now;
        ModificationDate = DateTime.Now;

    }

    #region [ Fields And Properties ]

    private string _iDPId;
    private string _nationalCode;
    private int? _companyId;
    private string _firstName;
    private string _lastName;
    private string _phoneNumber;
    private short _kYCStatus;
    private bool _isActive;
    private bool _isLegal;

    public string IDPId => _iDPId;
    public string NationalCode => _nationalCode;
    public int? CompanyId => _companyId;
    public string FirstName => _firstName;
    public string LastName => _lastName;
    public string PhoneNumber => _phoneNumber;
    public DateTime LastUpdateFromIDP { get; set; }
    public short KYCStatus => _kYCStatus;
    public bool IsLegal => _isActive;
    public bool IsActive => _isLegal;
    public DateTime CreatationDateTime { get; set; }
    public DateTime ModificationDate { get; set; }
    public List<UserRole> userRoles { get; set; } = new List<UserRole>();
    #endregion

    public static User Create(string iDPId, NationalCode nationalCode, Name name, PhoneNumber phoneNumber, short userRoleType)
    {
        var user = new User(iDPId, nationalCode.Value, name.FirstName, name.LastName, phoneNumber.Value);
        user.userRoles.Add(new UserRole(userRoleType));
       
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
}
