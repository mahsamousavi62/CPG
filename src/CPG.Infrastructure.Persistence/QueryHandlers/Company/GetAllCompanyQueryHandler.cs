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
            .Include(m => m.PaymentMethods)
            .ToListAsync(cancellationToken: cancellationToken);

        var companyViewModels = await Task.WhenAll(companies.Select(async company => new CompanyViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = await _minioProvider.PresignedGetObject(company.Logo),
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            CreationDate = company.CreationDate,
            ModificationDate = company.ModificationDate,
            IsActive = company.IsActive,
            PaymentMethods = company.PaymentMethods
          .ToDictionary(p => p.MethodType, p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString())
        })).ConfigureAwait(false);
        return companyViewModels;
    }

}
