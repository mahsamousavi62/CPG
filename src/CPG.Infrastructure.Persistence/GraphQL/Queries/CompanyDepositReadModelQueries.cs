using CPG.Application.UseCases.CompanyDeposits.ViewModels;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.DbContexts.ReadModels;
using CPG.Infrastructure.Persistence.GraphQL.Types.Company;
using CPG.Infrastructure.Persistence.GraphQL.Types.CompanyDeposit;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.GraphQL.Queries;

public class CompanyDepositReadModelQueries
{
 
    [UseOffsetPaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering<CompanyDepositFilterType>]
    [UseSorting<CompanayDepositSortType>]
    public IQueryable<CompanyDepositReadModel> GetCompanyDeposits([Service] ReadDbContext dbContext, [Service] IMinioProvider minioProvider)
    {
        var companyDeposits = dbContext.CompanyDepositReadModels.Include(c => c.Bank).Include(c => c.Company);
        var companyDepositViewModels =
             companyDeposits.Select(company => new CompanyDepositReadModel
             {
                 Id = company.Id,
                 Name = company.Name,
                 AccountNumber = company.AccountNumber,
                 Iban = company.Iban,
                 BankId = company.BankId,
                 BankLogo = minioProvider.PresignedGetObject(company.Bank.LogoAddress).GetAwaiter().GetResult(),
                 BankName = company.Bank.Name,
                 CompanyId = company.CompanyId,
                 CompanyName = company.Company.PersianName,
                 CreationDate = company.CreationDate,
                 IsActive = company.IsActive,
                 ModificationDate = company.ModificationDate,
             });
        return companyDepositViewModels;
    }
}
