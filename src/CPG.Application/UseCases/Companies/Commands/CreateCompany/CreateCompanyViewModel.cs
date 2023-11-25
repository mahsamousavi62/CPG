using CPG.Domain.SharedKernel.File;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Commands.CreateCompany;

public class CreateCompanyViewModel(string persianName, string englishName, bool nationalCodeMatchingRequied, IFile file, short[] methodTypes, List<long> users)
{
    public List<long> Users { get; set; } = users;
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public bool NationalCodeMatchingRequied { get; set; } = nationalCodeMatchingRequied;
    public IFile File { get; set; } = file;
    public short[] MethodTypes { get; set; } = methodTypes;
}