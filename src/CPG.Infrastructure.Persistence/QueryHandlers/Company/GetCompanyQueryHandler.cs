using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries
{
    public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, CompanyViewModel>
    {
        private readonly ReadDbContext _context;

        public GetCompanyQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyViewModel> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
        {
            var x = await _context.CompanyReadModels
                 .FirstOrDefaultAsync(t => t.Id == request.CompanyId);

            if (x == null) {
                return null;
            }

            var company = new CompanyViewModel
            {
                PersianName = x.PersianName,
                EnglishName = x.EnglishName,
                Logo = x.Logo,
                NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
                PaymentMethods = x.PaymentMethods
            };

            return company;
        }
    }
}
