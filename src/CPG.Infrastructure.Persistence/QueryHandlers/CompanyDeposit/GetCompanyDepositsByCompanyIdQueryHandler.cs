using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;

public class GetCompanyDepositsByCompanyIdQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyDepositsByCompanyIdQuery, Result<IReadOnlyCollection<CompanyDepositViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyDepositViewModel>>> Handle(GetCompanyDepositsByCompanyIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyDeposits = await _context.CompanyDepositReadModels
                .Include(t => t.Bank)
                .Include(t => t.Company)
                .Include(t => t.PaymentMethods)
                .Where(t => t.CompanyId == request.CompanyId)
                .ToListAsync();

            if (companyDeposits == null)
                throw new CompanyDepositByCompanyIdNotFoundException(request.CompanyId);

            var companyViewModels = await Task.WhenAll(
               companyDeposits.Select(async deposit => new CompanyDepositViewModel
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
                   PaymentMethods = deposit.PaymentMethods.Select(p => p.MethodType).ToList(),
               }))
               .ConfigureAwait(false);

            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.SuccessResult(companyViewModels);
        }
        catch (System.Exception ex)
        {
            return Result<IReadOnlyCollection<CompanyDepositViewModel>>.Failure(new Error("", ""));
        }
    }
}

