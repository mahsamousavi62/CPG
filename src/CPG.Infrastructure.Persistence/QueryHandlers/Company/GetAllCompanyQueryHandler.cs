using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Company;

public class GetAllCompanyQueryHandler : IRequestHandler<GetAllCompanyQuery, IReadOnlyCollection<CompanyViewModel>>
{
    private readonly ReadDbContext _context;

    public GetAllCompanyQueryHandler(ReadDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<CompanyViewModel>> Handle(GetAllCompanyQuery request, CancellationToken cancellationToken)
    {
        var companies = await _context.CompanyReadModels
            .Include(m => m.PaymentMethods)
            .ToListAsync(cancellationToken: cancellationToken);

        var companyViewModels = companies.Select(x => new CompanyViewModel
        {
            Id = x.Id,
            PersianName = x.PersianName,
            EnglishName = x.EnglishName,
            Logo = x.Logo,
            NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
            PaymentMethods = x.PaymentMethods
                .ToDictionary(p => p.MethodType, p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString())
        }).ToList();

        return companyViewModels;
    }

}
