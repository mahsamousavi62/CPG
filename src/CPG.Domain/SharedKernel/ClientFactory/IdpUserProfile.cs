using System;
using System.Collections.Generic;

namespace CPG.Domain.SharedKernel.ClientFactory;

public class Address
{
    public string PostalCode { get; set; }
    public string CountryName { get; set; }
    public string ProvinceName { get; set; }
    public string CityName { get; set; }
    public string SectionName { get; set; }
    public string CityPrefix { get; set; }
    public string RemnantAddress { get; set; }
    public string Alley { get; set; }
    public string Plaque { get; set; }
    public string Tel { get; set; }
    public object CountryPrefix { get; set; }
    public object Mobile { get; set; }
    public object EmergencyTel { get; set; }
    public object EmergencyTelCityPrefix { get; set; }
    public object Email { get; set; }
}

public class BankAccount
{
    public string AccountNumber { get; set; }
    public int BankId { get; set; }
    public int RayanId { get; set; }
    public long TadbirId { get; set; }
    public string Type { get; set; }
    public string Sheba { get; set; }
    public string BankTitle { get; set; }
    public string BranchCode { get; set; }
    public string BranchName { get; set; }
    public string BranchCityName { get; set; }
    public bool IsDefault { get; set; }
}

public class FinancialInfo
{
    public int AssetsValue { get; set; }
    public int InComingAverage { get; set; }
    public int SExchangeTransaction { get; set; }
    public int CExchangeTransaction { get; set; }
    public int OutExchangeTransaction { get; set; }
    public string TransactionLevel { get; set; }
    public string TradingKnowledgeLevel { get; set; }
    public object CompanyPurpose { get; set; }
    public object ReferenceRateCompany { get; set; }
    public object RateDate { get; set; }
    public int Rate { get; set; }
    public List<object> FinancialBrokers { get; set; }
}

public class JobInfo
{
    public DateTime EmploymentDate { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyPostalCode { get; set; }
    public string CompanyEmail { get; set; }
    public string CompanyWebSite { get; set; }
    public string CompanyCityPrefix { get; set; }
    public string CompanyPhone { get; set; }
    public object JobDescription { get; set; }
    public string Position { get; set; }
    public object CompanyFaxPrefix { get; set; }
    public object CompanyFax { get; set; }
    public string JobTitle { get; set; }
}

public class PrivatePerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FatherName { get; set; }
    public string Gender { get; set; }
    public string SeriShChar { get; set; }
    public string SeriSh { get; set; }
    public string Serial { get; set; }
    public string ShNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string PlaceOfIssue { get; set; }
    public string PlaceOfBirth { get; set; }
    public string FullName { get; set; }
}

public class Result
{
    public string Id { get; set; }
    public string UniqueIdentifier { get; set; }
    public long Mobile { get; set; }
    public object Email { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public PrivatePerson PrivatePerson { get; set; }
    public object legalPerson { get; set; }
    public List<Address> Addresses { get; set; }
    public List<TradingCode> TradingCodes { get; set; }
    public object Agent { get; set; }
    public List<BankAccount> BankAccounts { get; set; }
    public JobInfo JobInfo { get; set; }
    public FinancialInfo FinancialInfo { get; set; }
    public List<object> LegalPersonShareholders { get; set; }
    public List<object> LegalPersonStakeholders { get; set; }
}

public class IdpUserProfile
{
    public Result Result { get; set; }
}

public class TradingCode
{
    public string Type { get; set; }
    public string FirstPart { get; set; }
    public object SecondtPart { get; set; }
    public string ThirdPart { get; set; }
    public string Code { get; set; }
}

