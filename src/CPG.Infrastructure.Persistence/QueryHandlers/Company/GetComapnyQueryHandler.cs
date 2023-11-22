using CPG.Application.UseCases.Books.Queries;
using CPG.Application.UseCases.Books.ViewModels;
using CPG.Application.UseCases.Company.ViewModels;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Companies.Queries
{
    public class GetComapnyQueryHandler : IRequestHandler<GetComapnyQuery, CompanyViewModel>
    {
        private readonly ReadDbContext _context;

        public GetComapnyQueryHandler(ReadDbContext context)
        {
            _context = context;
        }
        public async Task<CompanyViewModel> Handle(GetComapnyQuery request, CancellationToken cancellationToken)
        {
            var book = await _context.CompanyViewModels.Where(c => c.Id == request.CompanyId)
                 .Select(x => new CompanyViewModel
                 {
                     PersianName = x.PersianName,
                     EnglishName = x.EnglishName,
                     Logo = x.Logo,
                     NationalCodeMatchingRequied = x.NationalCodeMatchingRequied,
                     PaymentMethods = x.PaymentMethods
                 })
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);

            return book;
        }
    }
}
