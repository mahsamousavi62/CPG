using CPG.Domain.SharedKernel.File;
using MediatR;

namespace CPG.Application.UseCases.Files.Commands.UploadFile;

public class UploadFileCommand(string uploadFromEntityType,IFile file) : IRequest<string>
{
    public string UploadFromEntityType { get; } = uploadFromEntityType;
    public IFile File { get; set; } = file;
}
