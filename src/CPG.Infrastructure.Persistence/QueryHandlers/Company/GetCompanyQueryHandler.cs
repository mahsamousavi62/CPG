using Ardalis.GuardClauses;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Companies.ViewModels;
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

        var company = await _context.CompanyReadModels.Include(x => x.PaymentMethods)
             .FirstOrDefaultAsync(t => t.Id == request.CompanyId, cancellationToken: cancellationToken) ?? throw new CompanyNotFoundException(request.CompanyId);

        var companyModel = new CompanyViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(company.Logo),
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            SiteAddress = company.SiteAddress,
            IpgRedirectionMethodType = company.IpgRedirectionMethodType,
            PaymentMethods = company.PaymentMethods.ToDictionary(p => p.MethodType,
                                    p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString()),
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive
        };

        return Result<CompanyViewModel>.SuccessResult(companyModel);
    }
}
