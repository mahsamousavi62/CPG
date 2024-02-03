using CPG.Application.UseCases.Providers.Commands.CreateProvider;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.File;

namespace CPG.Application.UseCases.Providers.ViewModels;

public class UpdateProviderViewModel : CreateProviderViewModel
{
    public UpdateProviderViewModel(long id ,string persianName, string englishName, Enums.ProviderType providerType, string providerData, IFile file, Enums.PaymentMethodType[] methodTypes) : base(persianName, englishName, providerType, providerData, file, methodTypes)
    {
       Id=id;
    }
    public long Id { get; set; }
}
