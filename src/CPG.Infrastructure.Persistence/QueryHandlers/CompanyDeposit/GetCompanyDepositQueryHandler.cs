using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit
{
    public class GetCompanyDepositQueryHandler :
        IRequestHandler<GetCompanyDepositQuery, CompanyDepositViewModel>
    {
        private readonly ReadDbContext _context;
        private readonly IMinioProvider _minioProvider;

        public GetCompanyDepositQueryHandler(ReadDbContext context, IMinioProvider minioProvider)
        {
            _context = context;
            _minioProvider = minioProvider;
        }

        public async Task<CompanyDepositViewModel> Handle(GetCompanyDepositQuery request, CancellationToken cancellationToken)
        {
            var companyDeposit = await _context.CompanyDepositReadModels.
            Include(c => c.Bank).Include(c => c.Company).FirstOrDefaultAsync(t => t.Id == request.CompanyDepositId);

            if (companyDeposit == null)
                throw new CompanyDepositNotFoundException(request.CompanyDepositId);

            var companyDepositViewModel = new CompanyDepositViewModel
            {
                Id = companyDeposit.Id,
                Name = companyDeposit.Name,
                AccountNumber = companyDeposit.AccountNumber,
                Iban = companyDeposit.Iban,
                BankId = companyDeposit.BankId,
                BankLogo = await _minioProvider.PresignedGetObject(companyDeposit.Bank.LogoAddress),
                BankName = companyDeposit.Bank.Name,
                CompanyId = companyDeposit.CompanyId,
                CompanyName = companyDeposit.Company.PersianName,
                CreationDate = companyDeposit.CreationDate,
                IsActive = companyDeposit.IsActive,
                ModificationDate = companyDeposit.ModificationDate,
            };
            return companyDepositViewModel;
        }
    }
}
