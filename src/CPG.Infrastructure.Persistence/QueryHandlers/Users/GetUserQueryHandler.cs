using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Companies.Exceptions;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users;
public class GetUserQueryHandler(ReadDbContext context) : IRequestHandler<GetUserQuery, UserViewModel>
{
    private readonly ReadDbContext _context = context;

    public async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.UserReadModels.Include(u => u.UserRoles).
                   SingleOrDefaultAsync(u => u.IsActive && u.IDPId == request.IdpId);

        if (user == null)
            throw new UserNotFoundException(request.IdpId);

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
        return userViewModel;
    }
}


