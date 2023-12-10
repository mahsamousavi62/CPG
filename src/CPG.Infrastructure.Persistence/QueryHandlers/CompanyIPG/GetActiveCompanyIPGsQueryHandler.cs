using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyIPG;

public class GetActiveCompanyIPGsQueryHandler(ReadDbContext context) : IRequestHandler<GetActiveCompanyIPGsQuery, IReadOnlyCollection<CompanyIPGViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<CompanyIPGViewModel>> Handle(GetActiveCompanyIPGsQuery request, CancellationToken cancellationToken)
    {
        return await _context.CompanyIPGReadModels
            .Include(m => m.CompanyIPGDeposits)
            .Where(c => c.IsActive && c.CompanyId == request.CompanyId)
            .Select(entity => new CompanyIPGViewModel
            {
                Id = entity.Id,
                CompanyId = entity.CompanyId,
                ProviderData = entity.ProviderData,
                IPGTypeId = entity.IPGTypeId,
                ProviderId = entity.ProviderId,
                IPGDeposits = entity.CompanyIPGDeposits.Select(t => new CompanyIPGDepositViewModel { Id = t.Id, CompanyDepositId = t.CompanyDepositId, CompanyIPGId = t.CompanyIPGId, IsDefault = t.IsDefault }).ToList(),
                CreationDate = entity.CreationDate,
                ModificationDate = entity.ModificationDate,
                IsActive = entity.IsActive,
            }).ToListAsync();
    }
}
