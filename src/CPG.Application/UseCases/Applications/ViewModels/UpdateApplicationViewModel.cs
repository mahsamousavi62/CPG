using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel.File;

namespace CPG.Application.UseCases.Applications.ViewModels;

public class UpdateApplicationViewModel : CreateApplicationViewModel
{
    public UpdateApplicationViewModel(int id ,string persianName, string englishName, string responseApiUrl, 
        string[] idpClientIds, string[] callbackUrls, IFile file)
        : base(persianName, englishName, responseApiUrl, idpClientIds, callbackUrls, file)
    {
        Id=id;
    }
    public int Id { get; set; }
}
