using Ardalis.GuardClauses;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
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
            Guard.Against.NegativeOrZero(request.CompanyId, nameof(request.CompanyId));

            var x = await _context.CompanyReadModels.Include(x => x.PaymentMethods)
                 .FirstOrDefaultAsync(t => t.Id == request.CompanyId);

            if (x == null)
                throw new CompanyNotFoundException(request.CompanyId);

            var company = new CompanyViewModel
            {
                Id = x.Id,
                PersianName = x.PersianName,
                EnglishName = x.EnglishName,
                Logo = x.Logo,
                NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
                PaymentMethods = x.PaymentMethods.ToDictionary(p => p.MethodType,
                                        p => ((Enums.CompanyPaymentMethodType)p.MethodType).ToString())
            };

            return company;
        }
    }
}
