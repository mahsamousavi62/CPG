using CPG.Domain.SharedKernel.File;

namespace CPG.Application.UseCases.Application.Commands.CreateApplication;

public class CreateApplicationViewModel(string persianName, string englishName, string responseApiUrl, string[] idpClientIds, string[] callbackUrls, IFile file)
{
    public string PersianName { get; set; } = persianName;
    public string EnglishName { get; set; } = englishName;
    public string ResponseApiUrl { get; set; } = responseApiUrl;
    public IFile File { get; set; } = file;
    public string[] IdpClientIds { get; set; } = idpClientIds;
    public string[] CallbackUrls { get; set; } = callbackUrls;
}