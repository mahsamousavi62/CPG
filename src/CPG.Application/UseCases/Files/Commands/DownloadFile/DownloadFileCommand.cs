using CPG.Domain.SharedKernel.File;
using MediatR;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommand : IRequest<FileViewModel>
{
    public string FileName { get; set; }

    public DownloadFileCommand(string fileName)
    {
        FileName = fileName;
    }
}
