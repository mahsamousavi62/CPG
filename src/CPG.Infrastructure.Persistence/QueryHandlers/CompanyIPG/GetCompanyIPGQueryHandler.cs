using Ardalis.GuardClauses;
using CPG.Application.UseCases.CompanyIPGs.Exceptions;
using CPG.Application.UseCases.CompanyIPGs.Queries;
using CPG.Application.UseCases.CompanyIPGs.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyIPG;

public class GetCompanyIPGQueryHandler(ReadDbContext context) : IRequestHandler<GetCompanyIPGQuery, CompanyIPGViewModel>
{
    private readonly ReadDbContext _context = context;
    
    public async Task<CompanyIPGViewModel> Handle(GetCompanyIPGQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.CompanyIPGId, nameof(request.CompanyIPGId));

        var entity = await _context.CompanyIPGReadModels.Include(x => x.CompanyIPGDeposits).FirstOrDefaultAsync(t => t.Id == request.CompanyIPGId);

        if (entity == null)
            throw new CompanyIPGNotFoundException(request.CompanyIPGId);

        return new CompanyIPGViewModel
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
        };
    }
}