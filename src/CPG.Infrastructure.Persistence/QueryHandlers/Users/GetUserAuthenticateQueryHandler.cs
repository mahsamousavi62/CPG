using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users
{
    public class GetUserAuthenticateQueryHandler(ReadDbContext context, IAuthenticationService authenticationService, IRedisCacheService cacheService) : IRequestHandler<GetUserAuthenticateQuery, UserAuthenticateViewModel>
    {
        private readonly ReadDbContext _context = context;
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly IRedisCacheService _cacheService = cacheService;
        public const string CacheKey = "CurrntUser_key";
        public async Task<UserAuthenticateViewModel> Handle(GetUserAuthenticateQuery request, CancellationToken cancellationToken)
        {
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);
            var clientId = await _authenticationService.GetClientId(string.Empty);

            var user = await _context.UserReadModels.Include(u => u.UserRoles).
                       SingleOrDefaultAsync(u => u.IsActive && u.IDPId == sub);

            var applicationIdentifier = (await _context.ApplicationIdentifierReadModels.SingleOrDefaultAsync(a => a.IdpClientId == clientId));

            var userModel = new UserAuthenticateViewModel
            {
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                Id = user?.Id ?? 0,
                NationalCode = user?.NationalCode ?? string.Empty,
                IDPId = clientId,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                UserRoles = user?.UserRoles.ToDictionary(p => p.RoleType, p => ((Enums.UserRoleType)p.RoleType).ToString()),
                CompanyId = user?.CompanyId ?? 0,
                ApplicationId = applicationIdentifier?.ApplicationId ?? 0,
            };

            return userModel;
        }
    }
}

