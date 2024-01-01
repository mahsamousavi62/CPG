using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Application.UseCases.CompanyIPGs.Queries;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.SharedKernel.Minio;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyIPG;

public class GetCompanyIPGDepositsQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyIPGDepositsQuery, IReadOnlyCollection<CompanyDepositViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyCollection<CompanyDepositViewModel>> Handle(GetCompanyIPGDepositsQuery request, CancellationToken cancellationToken)
    {
        var depositIds = await _context.CompanyIPGDepositReadModels.Where(t => t.CompanyIPGId == request.CompanyIPGId).Select(t => t.CompanyDepositId).ToListAsync();
        var deposits = await _context.CompanyDepositReadModels.Where(t => depositIds.Contains(t.Id)).Include(t => t.Bank).Include(t => t.Company).ToListAsync();

        return await Task.WhenAll(deposits.Select(async deposit => new CompanyDepositViewModel
        {
            CompanyId = deposit.CompanyId,
            AccountNumber = deposit.AccountNumber,
            BankId = deposit.BankId,
            BankName = deposit.Bank.Name,
            CompanyName = deposit.Company.PersianName,
            Iban = deposit.Iban,
            Id = deposit.Id,
            IsActive = deposit.IsActive,
            Name = deposit.Name,
            CreationDate = deposit.CreationDate,
            ModificationDate = deposit.ModificationDate,
            BankLogo = await _minioProvider.PresignedGetObject(deposit.Bank.LogoAddress),
        })).ConfigureAwait(false);
    }
}