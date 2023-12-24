using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyViewModel(string persianName, string englishName, bool nationalCodeMatchingRequied, 
    IFile file, short[] methodTypes, List<long> users, string siteAddress, short ipgRedirectionMethodType)
{
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public bool NationalCodeMatchingRequied { get; set; } = nationalCodeMatchingRequied;
    public string SiteAddress { get; set; } = siteAddress;
    public short IpgRedirectionMethodType { get; set; }=ipgRedirectionMethodType;
    public IFile File { get; set; } = file;
    public short[] MethodTypes { get; set; } = methodTypes;
    public List<long> Users { get; set; } = users;
}