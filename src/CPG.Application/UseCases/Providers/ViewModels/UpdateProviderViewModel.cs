using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Providers.ViewModels;

public class UpdateProviderViewModel(long id, string persianName, string englishName,
         string providerData, IFile file, Enums.PaymentMethodType[] methodTypes)   
{
    public long Id { get; set; } = id;
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public string ProviderData { get; set; } = providerData;
    public IFile File { get; set; } = file;
    public PaymentMethodType[] MethodTypes { get; set; } = methodTypes;
}
