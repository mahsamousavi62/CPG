using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company;

public class GetAllCompanyQueryHandler(ReadDbContext context, IMinioProvider minioProvider)
    : IRequestHandler<GetAllCompanyQuery, Result<IReadOnlyCollection<CompanyViewModel>>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<IReadOnlyCollection<CompanyViewModel>>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
    {
             var companies = await _context.CompanyReadModels
            .Include(c => c.PaymentMethods).Include(c => c.Users)
            .ToListAsync(cancellationToken: cancellationToken);

        var companyViewModels = await Task.WhenAll(companies.Select(async company => new CompanyViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = await General.GetLogo(_minioProvider, company.Logo),
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            SiteAddress = company.SiteAddress,
            Code = company.Code,
            IpgRedirectionMethodType = company.IpgRedirectionMethodType,
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive,
            PaymentMethods = company.PaymentMethods.Select(p => p.MethodType).ToList(),
            Users = company.Users?.Select(u => new UserCompanyViewModel { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, NationalCode = u.NationalCode }).ToList()

        })).ConfigureAwait(false);
       
        return Result<IReadOnlyCollection<CompanyViewModel>>.SuccessResult(companyViewModels);
    }
}
