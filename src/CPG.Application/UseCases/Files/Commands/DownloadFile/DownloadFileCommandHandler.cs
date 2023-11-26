using Ardalis.GuardClauses;
using CPG.Domain.SharedKernel.File;
using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Files.Commands.DownloadFile;

public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, string>
{
    private readonly IMinioProvider _minioProvider;

    public DownloadFileCommandHandler(IMinioProvider minioProvider)
    {
        _minioProvider = minioProvider;
    }
    public async Task<string> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request.FileUrl, nameof(request.FileUrl));
        return await _minioProvider.PresignedGetObject(request.FileUrl);
    }
}
