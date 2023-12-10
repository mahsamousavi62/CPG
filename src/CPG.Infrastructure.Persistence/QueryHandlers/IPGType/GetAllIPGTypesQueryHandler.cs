using CPG.Application.UseCases.IPGType.Queries;
using CPG.Application.UseCases.IPGType.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.IPGType;

public class GetAllIPGTypesQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllIPGTypesQuery, IReadOnlyCollection<IPGTypeViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyCollection<IPGTypeViewModel>> Handle(GetAllIPGTypesQuery request, CancellationToken cancellationToken)
    {
        var ipgTypes = await _context.IPGTypeReadModels.ToListAsync(cancellationToken: cancellationToken);

        return await Task.WhenAll(ipgTypes.Select(async x => new IPGTypeViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(x.Logo),            
            CreationDate = x.CreationDate,
            ModificationDate = x.ModificationDate,
        })).ConfigureAwait(false);
    }
}