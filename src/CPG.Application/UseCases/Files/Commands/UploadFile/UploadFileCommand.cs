using CPG.Domain.SharedKernel.File;
using MediatR;

namespace CPG.Application.UseCases.Files.Commands.UploadFile;

public class UploadFileCommand(IFile file) : IRequest<string>
{
    public IFile File { get; set; } = file;
}
