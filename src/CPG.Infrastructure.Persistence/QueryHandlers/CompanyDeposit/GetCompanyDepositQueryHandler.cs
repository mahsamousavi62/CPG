using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.CompanyDeposits.Queries;
using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.CompanyDeposit;

public class GetCompanyDepositQueryHandler(ReadDbContext context, IMinioProvider minioProvider) :
    IRequestHandler<GetCompanyDepositQuery, Result<CompanyDepositViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<CompanyDepositViewModel>> Handle(GetCompanyDepositQuery request, CancellationToken cancellationToken)
    {
        var companyDeposit = await _context.CompanyDepositReadModels.Include(c => c.Bank)
            .Include(c => c.Company)
            .FirstOrDefaultAsync(t => t.Id == request.CompanyDepositId);

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
            IsDefaultForDD = companyDeposit.IsDefaultForDD,
            CreationDate = companyDeposit.CreationDate,
            IsActive = companyDeposit.IsActive,
            ModificationDate = companyDeposit.ModificationDate,
        };

        return Result<CompanyDepositViewModel>.SuccessResult(companyDepositViewModel);
    }
}
