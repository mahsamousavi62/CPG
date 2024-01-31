using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel.File;

namespace CPG.Application.UseCases.Applications.ViewModels;

public class UpdateApplicationViewModel : CreateApplicationViewModel
{
    public UpdateApplicationViewModel(string persianName, string englishName, string responseApiUrl, 
        string[] idpClientIds, string[] callbackUrls, IFile file)
        : base(persianName, englishName, responseApiUrl, idpClientIds, callbackUrls, file)
    {
    }
    public long Id { get; set; }
}
