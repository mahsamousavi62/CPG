using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;

public class GetAllCompanyDepositQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetAllCompanyDepositQuery, Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> Handle(GetAllCompanyDepositQuery request, CancellationToken cancellationToken)
    {
        var companyDeposits = await _context.CompanyDepositReadModels
            .Include(c => c.Bank)
            .Include(c => c.Company)
            .ToListAsync(cancellationToken);

        var companyDepositViewModels = await Task.WhenAll( 
            companyDeposits.Select(async company => new CompanyDepositViewModel
            {
                Id = company.Id,
                Name = company.Name,
                AccountNumber = company.AccountNumber,
                Iban = company.Iban,
                BankId=company.BankId,  
                BankLogo = await _minioProvider.PresignedGetObject(company.Bank.LogoAddress),
                BankName = company.Bank.Name,
                CompanyId = company.CompanyId,
                CompanyName = company.Company.PersianName,
                IsDefaultForDirectDebit = company.IsDefaultForDirectDebit,
                CreationDate = company.CreationDate,
                IsActive = company.IsActive,
                ModificationDate = company.ModificationDate,
            }))
            .ConfigureAwait(false);

        return Result<IReadOnlyCollection<CompanyDepositViewModel>>.SuccessResult(companyDepositViewModels);
    }
}
