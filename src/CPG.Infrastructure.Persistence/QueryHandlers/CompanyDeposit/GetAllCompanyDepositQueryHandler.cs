using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CPG.Application.UseCases.Companies.ViewModels;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit
{
    public class GetAllCompanyDepositQueryHandler : IRequestHandler<GetAllCompanyDepositQuery, IReadOnlyCollection<CompanyDepositViewModel>>
    {
        private readonly ReadDbContext _context;
        private readonly IMinioProvider _minioProvider;

        public GetAllCompanyDepositQueryHandler(ReadDbContext context, IMinioProvider minioProvider)
        {
            _context = context;
            _minioProvider = minioProvider;
        }

        public async Task<IReadOnlyCollection<CompanyDepositViewModel>> Handle(GetAllCompanyDepositQuery request, CancellationToken cancellationToken)
        {
            var companyDeposits = await _context.CompanyDepositReadModels.Include(c => c.Bank).Include(c => c.Company).ToListAsync(cancellationToken);

            var companyViewModels = await Task.WhenAll( 
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
                CreationDate = company.CreationDate,
                IsActive = company.IsActive,
                ModificationDate = company.ModificationDate,
            })).ConfigureAwait(false);
            return companyViewModels.ToList();
        }
    }
}
