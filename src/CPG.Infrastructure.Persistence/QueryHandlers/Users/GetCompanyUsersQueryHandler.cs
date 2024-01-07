using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users
{
    public class GetCompanyUsersQueryHandler : IRequestHandler<GetCompanyUsersQuery, IReadOnlyCollection<UserCompanyViewModel>>
    {
        private readonly ReadDbContext _context;

        public GetCompanyUsersQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<UserCompanyViewModel>> Handle(GetCompanyUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _context.UserReadModels.
                Where(u => u.KYCStatus == 1 && u.IsActive && u.CompanyId == null).ToListAsync();

            var userViewModels = users.Select(user => new UserCompanyViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                NationalCode = user.NationalCode,
            }).ToList();
            return userViewModels;
        }
    }
}
