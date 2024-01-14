using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Providers.Commands.CreateProvider;

public class CreateProviderViewModel(string persianName, string englishName, ProviderType providerType, 
    string providerData, IFile file, PaymentMethodType[] methodTypes)
{
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public ProviderType ProviderType { get; set; } = providerType;
    public string ProviderData { get; set; } = providerData;
    public IFile File { get; set; } = file;
    public PaymentMethodType[] MethodTypes { get; set; } = methodTypes;
}