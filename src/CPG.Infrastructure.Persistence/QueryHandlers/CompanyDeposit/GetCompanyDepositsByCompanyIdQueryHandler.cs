using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;
public class GetCompanyDepositsByCompanyIdQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyDepositsByCompanyIdQuery, IReadOnlyList<CompanyDepositViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<IReadOnlyList<CompanyDepositViewModel>> Handle(GetCompanyDepositsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        var companyDeposits = await _context.CompanyDepositReadModels.
            Include(c => c.Bank).Include(c => c.Company)
            .Where(t => t.CompanyId == request.CompanyId).ToListAsync();

        if (companyDeposits == null)
            throw new CompanyDepositByCompanyIdNotFoundException(request.CompanyId);

        var companyViewModels = await Task.WhenAll(
           companyDeposits.Select(async company => new CompanyDepositViewModel
           {
               Id = company.Id,
               Name = company.Name,
               AccountNumber = company.AccountNumber,
               Iban = company.Iban,
               BankId = company.BankId,
               BankLogo = await _minioProvider.PresignedGetObject(company.Bank.LogoAddress),
               BankName = company.Bank.Name,
               CompanyId = company.CompanyId,
               CompanyName = company.Company.PersianName,
               CreationDate = company.CreationDate,
               IsActive = company.IsActive,
               ModificationDate = company.ModificationDate,
           }))
           .ConfigureAwait(false);

        return companyViewModels.ToList();
    }
}

