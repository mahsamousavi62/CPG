using Ardalis.GuardClauses;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries;

public class GetCompanyQueryHandler(ReadDbContext context) : IRequestHandler<GetCompanyQuery, CompanyViewModel>
{
    private readonly ReadDbContext _context = context;

        public async Task<CompanyViewModel> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            Guard.Against.NegativeOrZero(request.CompanyId, nameof(request.CompanyId));

            var company = await _context.CompanyReadModels.Include(x => x.PaymentMethods)
                 .FirstOrDefaultAsync(t => t.Id == request.CompanyId);

            if (company == null)
                throw new CompanyNotFoundException(request.CompanyId);

            var companyModel = new CompanyViewModel
            {
                Id = company.Id,
                PersianName = company.PersianName,
                EnglishName = company.EnglishName,
                Logo = company.Logo,
                NationalCodeMatchingRequied = company.NationalCodeMatchingRequied,
                PaymentMethods = company.PaymentMethods.ToDictionary(p => p.MethodType,
                                        p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString())
            };

        return companyModel;
    }
}
