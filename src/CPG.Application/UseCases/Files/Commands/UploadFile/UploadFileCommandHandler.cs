using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Files.Commands.UploadFile;

public class UploadFileCommandHandler(IMinioProvider minioProvider) : IRequestHandler<UploadFileCommand, string>
{
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<string> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var result=await _minioProvider.PutObject(request.UploadFromEntityType, request.File);
        return result;
    }
}
