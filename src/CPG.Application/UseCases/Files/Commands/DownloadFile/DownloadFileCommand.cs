using CPG.Domain.SharedKernel.File;
using MediatR;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommand : IRequest<IFile>
{
    public string FileName { get; set; }

    public DownloadFileCommand(string fileName)
    {
        FileName = fileName;
    }
}
