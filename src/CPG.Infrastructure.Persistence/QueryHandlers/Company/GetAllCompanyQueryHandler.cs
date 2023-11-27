using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company;

public class GetAllCompanyQueryHandler : IRequestHandler<GetAllCompanyQuery, IReadOnlyCollection<CompanyViewModel>>
{
    private readonly ReadDbContext _context;
    private readonly IMinioProvider _minioProvider;
    public GetAllCompanyQueryHandler(ReadDbContext context, IMinioProvider minioProvider)
    {
        _context = context;
        _minioProvider = minioProvider;
    }

    public async Task<IReadOnlyCollection<CompanyViewModel>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
    {
        var companies = await _context.CompanyReadModels
            .Include(m => m.PaymentMethods).Where(c=>c.IsActive)
            .ToListAsync(cancellationToken: cancellationToken);

        var companyViewModels = await Task.WhenAll(companies.Select(async x => new CompanyViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(x.Logo),
            NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
            PaymentMethods = x.PaymentMethods
          .ToDictionary(p => p.MethodType, p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString())
        })).ConfigureAwait(false);
        return companyViewModels;
    }

}
