using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetAllCompaniesQueryHandler(ReadDbContext context) : IRequestHandler<GetAllCompaniesQuery, IReadOnlyCollection<CompanyViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<IReadOnlyCollection<CompanyViewModel>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = await _context.CompanyReadModels
             .Select(x => new CompanyViewModel
             {
                 Id = x.Id,
                 PersianName = x.PersianName,
                 EnglishName = x.EnglishName,
                 Logo = x.Logo,
                 NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
                 PaymentMethods = x.PaymentMethods
             })
            .ToListAsync(cancellationToken: cancellationToken);

        return companies;
    }
}
