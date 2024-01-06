using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users
{
    public class GetUserAuthenticateQueryHandler(ReadDbContext context, IAuthenticationService authenticationService) : IRequestHandler<GetUserAuthenticateQuery, UserAuthenticateViewModel>
    {
        private readonly ReadDbContext _context = context;
        private readonly IAuthenticationService _authenticationService = authenticationService;

        public async Task<UserAuthenticateViewModel> Handle(GetUserAuthenticateQuery request, CancellationToken cancellationToken)
        {
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);
            var clientId = await _authenticationService.GetClientId( string.Empty);

            var user = await _context.UserReadModels.Include(u => u.UserRoles).
                       SingleOrDefaultAsync(u => u.IsActive && u.IDPId == sub);
            if (user == null)
                return null;
            var applicationIdentifier = (await _context.ApplicationIdentifierReadModels.
                SingleOrDefaultAsync(a => a.IdpClientId == clientId));
            //todo :optimize 
           var auditType = applicationIdentifier?.IdpClientId != "pay__daryaftyar_client"
                ? Enums.AuditType.Client : Enums.AuditType.User;

            var userViewModel = new UserAuthenticateViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                NationalCode = user.NationalCode,
                IDPId = user.IDPId,
                PhoneNumber = user.PhoneNumber,
                UserRoles = user.UserRoles.ToDictionary(p => p.RoleType, p => ((Enums.UserRoleType)p.RoleType).ToString()),
                CompanyId = user.CompanyId,
                ApplicationId = applicationIdentifier.ApplicationId,
                AuditType = auditType
            };
            return userViewModel;
        }
    }
}

