using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;


public class CreateCompanyViewModel(string persianName, string englishName, bool nationalCodeMatchingRequied, 
    IFile file, Enums.PaymentMethodType[] methodTypes, List<long> users, string siteAddress, Enums.IpgRedirectionMethodType ipgRedirectionMethodType,
    string key, string iv, int? thirdPartyCode)
{
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public bool NationalCodeMatchingRequied { get; set; } = nationalCodeMatchingRequied;
    public string SiteAddress { get; set; } = siteAddress;
    public Enums.IpgRedirectionMethodType IpgRedirectionMethodType { get; set; } = ipgRedirectionMethodType;
    public IFile File { get; set; } = file;
    public Enums.PaymentMethodType[] MethodTypes { get; set; } = methodTypes;
    public List<long> Users { get; set; } = users;
    public string Key { get; set; } = key;
    public string IV { get; set; } = iv;
    public int? ThirdPartyCode { get; set; } = thirdPartyCode;
}