using CPG.Application.UseCases.IPGTypes.Queries;
using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.IPGType;

public class GetAllIPGTypesQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllIPGTypesQuery, Result<IReadOnlyCollection<IPGTypeViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<IPGTypeViewModel>>> Handle(GetAllIPGTypesQuery request, CancellationToken cancellationToken)
    {
        var ipgTypes = await _context.IPGTypeReadModels.ToListAsync(cancellationToken: cancellationToken);

        var viewModels = await Task.WhenAll(ipgTypes.Select(async x => new IPGTypeViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await General.GetLogo( _minioProvider,x.Logo),            
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<IPGTypeViewModel>>.SuccessResult(viewModels);
    }
}