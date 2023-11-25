using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Files.Commands.UploadFile;

public class UploadPhotoCommandHandler : IRequestHandler<UploadPhotoCommand, string>
{
    private readonly IMinioProvider _minioProvider;

    public UploadPhotoCommandHandler(IMinioProvider minioProvider)
    {
        _minioProvider = minioProvider;
    }

    public async Task<string> Handle(UploadPhotoCommand request, CancellationToken cancellationToken)
    {
        var result=await _minioProvider.PutObject(request.File);
        return result;
    }
}
