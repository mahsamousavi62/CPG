using Ardalis.GuardClauses;
using CPG.Application.UseCases.IPGTypes.Exceptions;
using CPG.Application.UseCases.IPGTypes.Queries;
using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.IPGType;

public class GetIPGTypeQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetIPGTypeQuery, Result<IPGTypeViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IPGTypeViewModel>> Handle(GetIPGTypeQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.IPGTypeId, nameof(request.IPGTypeId));

        var ipgType = await _context.IPGTypeReadModels.FirstOrDefaultAsync(t => t.Id == request.IPGTypeId);

        if (ipgType == null)
            throw new IPGTypeNotFoundException(request.IPGTypeId);

        var ipgTypeModel = new IPGTypeViewModel
        {
            Id = ipgType.Id,
            PersianName = ipgType.PersianName,
            EnglishName = ipgType.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(ipgType.Logo),
            Code = ipgType.Code,
            IsActive = ipgType.IsActive,
        };

        return Result<IPGTypeViewModel>.SuccessResult(ipgTypeModel);
    }
}