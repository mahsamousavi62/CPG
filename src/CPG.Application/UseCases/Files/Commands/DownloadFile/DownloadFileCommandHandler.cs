using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, FileViewModel>
{
    private readonly IMinioProvider _minioProvider;

    public DownloadFileCommandHandler(IMinioProvider minioProvider)
    {
        _minioProvider = minioProvider;
    }
    public async Task<FileViewModel> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
    {
        var result = await _minioProvider.GetObjectByName(request.FileName);
    
    var result2=await _minioProvider.PresignedGetObject(request.FileName);
        return result;

    }
}
