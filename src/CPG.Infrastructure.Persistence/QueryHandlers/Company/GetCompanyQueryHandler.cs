using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQueryHandler(ReadDbContext context) : IRequestHandler<GetCompanyQuery, CompanyViewModel>
{
    private readonly ReadDbContext _context = context;

    public async Task<CompanyViewModel> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.CompanyReadModels
             .FirstOrDefaultAsync(t => t.Id == request.CompanyId);

        if (company == null) {
            return null;
        }

        var companyModel = new CompanyViewModel
        {
            Id = company.Id,
            PersianName = company.PersianName,
            EnglishName = company.EnglishName,
            Logo = company.Logo,
            NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
            PaymentMethods = company.PaymentMethods
        };

        return companyModel;
    }
}
