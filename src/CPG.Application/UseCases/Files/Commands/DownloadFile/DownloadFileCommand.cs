using CPG.Domain.SharedKernel.File;
using MediatR;
using System.IO.Enumeration;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommand : IRequest<string>
{
    public string FileUrl { get; set; }

    public DownloadFileCommand(string fileUrl)
    {
        FileUrl = fileUrl;
    }
}
