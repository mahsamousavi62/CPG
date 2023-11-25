using CPG.Domain.SharedKernel.File;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Companies.Commands.Create;

public class CreateCompanyViewModel
{

    public CreateCompanyViewModel(string persianName, string englishName, bool nationalCodeMatchingRequied, IFile file, short[] methodTypes, List<long> users)
    {
        PersianName = persianName;
        EnglishName = englishName;
        NationalCodeMatchingRequied = nationalCodeMatchingRequied;
        File = file;
        MethodTypes = methodTypes;
        Users = users;
    }

    public List<long> Users { get; set; }
    public string PersianName { get; set; }
    public string EnglishName { get; set; }
    public bool NationalCodeMatchingRequied { get; set; }
    public IFile File { get; set; }
    public short[] MethodTypes { get; set; }
}