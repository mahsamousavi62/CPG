using Ardalis.GuardClauses;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQueryHandler(ReadDbContext context, IMinioProvider minioProvider) : IRequestHandler<GetCompanyQuery, Result<CompanyViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<CompanyViewModel>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.NegativeOrZero(request.CompanyId, nameof(request.CompanyId));

        var company = await _context.CompanyReadModels.Include(c => c.PaymentMethods).Include(c => c.Users)
             .FirstOrDefaultAsync(t => t.Id == request.CompanyId, cancellationToken: cancellationToken) ?? throw new CompanyNotFoundException(request.CompanyId);

        var companyModel = new CompanyViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = await General.GetLogo(_minioProvider, company.Logo),
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            SiteAddress = company.SiteAddress,
            IpgRedirectionMethodType = company.IpgRedirectionMethodType,
            Code = company.Code,
            PaymentMethods = company.PaymentMethods.Select(p => p.MethodType).ToList(),
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive,
            Users = company.Users?.Select(u =>
            new UserCompanyViewModel { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, NationalCode = u.NationalCode }).ToList()
        };
        return Result<CompanyViewModel>.SuccessResult(companyModel);
    }
}
