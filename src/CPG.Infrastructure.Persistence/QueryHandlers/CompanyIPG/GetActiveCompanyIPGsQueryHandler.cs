using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyIPG;

public class GetActiveCompanyIPGsQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetActiveCompanyIPGsQuery, Result<IReadOnlyCollection<CompanyIPGDataViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyIPGDataViewModel>>> Handle(GetActiveCompanyIPGsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.CompanyIPGReadModels
            .Include(t => t.CompanyIPGDeposits)
            .ThenInclude(t => t.CompanyDeposit)
            .Include(t => t.IPGType)
            .Include(t => t.Provider)
            .Where(t => t.CompanyId == request.CompanyId && t.IsActive).ToListAsync();

        var viewModels = await Task.WhenAll(data.Select(async entity => new CompanyIPGDataViewModel
        {
            Id = entity.Id,
            CompanyId = entity.CompanyId,
            ProviderData = entity.ProviderData,
            IPGTypeId = entity.IPGTypeId,
            ProviderId = entity.ProviderId,
            Deposits = entity.CompanyIPGDeposits.Select(t => new CompanyIPGDepositDataViewModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).ToList(),
            CreationDate = entity.CreationDate,
            ModificationDate = entity.ModificationDate,
            IsActive = entity.IsActive,
            DefaultDeposit = entity.CompanyIPGDeposits.Where(t => t.IsDefault == true).Select(t => new CompanyIPGDepositDataViewModel { Id = t.Id, AccountNumber = t.CompanyDeposit.AccountNumber, Name = t.CompanyDeposit.Name }).FirstOrDefault(),
            IPGTypeLogo = await _minioProvider.PresignedGetObject(entity.IPGType.Logo),
            IPGTypeName = entity.IPGType.PersianName,
            ProviderName = entity.Provider.PersianName,
        })).ConfigureAwait(false);

        return Result<IReadOnlyCollection<CompanyIPGDataViewModel>>.SuccessResult(viewModels);
    }
}
