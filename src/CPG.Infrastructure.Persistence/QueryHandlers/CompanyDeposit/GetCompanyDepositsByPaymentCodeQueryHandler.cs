using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;

public class GetCompanyDepositsByPaymentCodeQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyDepositsByPaymentCodeQuery, Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> Handle(GetCompanyDepositsByPaymentCodeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequestCompanyId = await _context.PaymentRequestReadModels.Where(t => t.PaymentCode == request.PaymentCode)
                .Select(t => t.CompanyId).FirstOrDefaultAsync();
            var companyDeposits = await _context.CompanyDepositReadModels.Include(t => t.Company).Include(c => c.Bank).Where(t => t.CompanyId == paymentRequestCompanyId)
                .ToListAsync();

            var companyViewModels = await Task.WhenAll(
               companyDeposits?.Select(async deposit => new CompanyDepositViewModel
               {
                   Id = deposit.Id,

                   Name = deposit.Name,
                   AccountNumber = deposit.AccountNumber,
                   Iban = deposit.Iban,
                   BankId = deposit.BankId,
                   BankLogo = await _minioProvider.PresignedGetObject(deposit.Bank.Logo),
                   BankName = deposit.Bank.Name,
                   CompanyId = deposit.CompanyId,
                   CompanyName = deposit.Company.PersianName,
                   IsDefaultForDirectDebit = deposit.IsDefaultForDirectDebit,
                   CreationDate = deposit.CreationDate,
                   IsActive = deposit.IsActive,
                   ModificationDate = deposit.ModificationDate,
               }))
               .ConfigureAwait(false);

            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.SuccessResult(companyViewModels);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.Failure(new Error(ex.Source, ex.Message));
        }
    }
}
