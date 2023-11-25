using CPG.Domain.SharedKernel.File;
using MediatR;


namespace CPG.Application.UseCases.Files.Commands.UploadFile
{
    public class UploadPhotoCommand:IRequest<string>
    {
        public IFile File { get; set; }

        public UploadPhotoCommand(IFile file)
        {
            File = file;
        }
    }
}
