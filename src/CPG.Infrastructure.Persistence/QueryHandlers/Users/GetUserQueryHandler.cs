using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users;

public class GetUserQueryHandler(ReadDbContext context, IAuthenticationService authenticationService) : IRequestHandler<GetUserQuery, Result<UserViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<Result<UserViewModel>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
		try
		{
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);

            var user = await _context.UserReadModels.Include(u => u.UserRoles)
                .SingleOrDefaultAsync(u => u.IsActive && u.IDPId == sub);
            var applicationId = (await _context.ApplicationIdentifierReadModels.SingleOrDefaultAsync(a => a.IdpClientId == sub))?.ApplicationId;
            if (user == null)
                throw new UserNotFoundException(sub);

            var userViewModel = new UserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                NationalCode = user.NationalCode,
                IDPId = user.IDPId,
                PhoneNumber = user.PhoneNumber,
                UserRoles = user.UserRoles.ToDictionary(p => p.RoleType,
                                        p => ((Enums.UserRoleType)p.RoleType).ToString()),
            };
            return Result<UserViewModel>.SuccessResult(userViewModel);
        }
        catch (Exception exc)
        {
            if (exc is DomainException )
                return Result<UserViewModel>.Failure(new Error((exc as dynamic).Code, exc.Message));
            else
                return Result<UserViewModel>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}


