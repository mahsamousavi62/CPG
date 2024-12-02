using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.UserAggregate;
using CPG.Domain.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Communication.Idp;
using CPG.Domain.SharedKernel.Interfaces;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users;

public class GetUserQueryHandler(
                                ReadDbContext context,
                                IAuthenticationService authenticationService,
                                IMinioProvider minioProvider,
                                IIdpProvider idpClient)
                                : IRequestHandler<GetUserQuery, Result<UserViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly string userKycStatus = "KycVerified";
    private readonly string demo = "Demo";
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly IIdpProvider _idpClient = idpClient;

    public async Task<Result<UserViewModel>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);

        var kycStatus = await _authenticationService.GetDataFromClaim<string>("status", string.Empty);

        if (kycStatus != userKycStatus && kycStatus != demo)
        {
            var idpUserProfileResponse = await _idpClient.GetUserStatus(sub);

            if (idpUserProfileResponse.OperationResult is not Enums.OperationResult.Succeeded
            || idpUserProfileResponse.Data?.Result?.Status != userKycStatus)
                throw new UserNotVerifyStatusException(string.Empty);
        }

        var user = await _context.UserReadModels.Include(u => u.UserRoles).Include(c => c.Company)
                                                        .SingleOrDefaultAsync(u => u.IsActive && u.IDPId == sub);
        if (user != null)
        {
            var applicationId = (await _context.ApplicationIdentifierReadModels.SingleOrDefaultAsync(a => a.IdpClientId == sub))?.ApplicationId;
        }

        var userViewModel = new UserViewModel
        {
            CompanyId = user?.Company?.Id,
            CompanyPersianName = user?.Company?.PersianName,
            CompanyLogo = !string.IsNullOrEmpty(user?.Company?.Logo) ? await General.GetLogo(minioProvider, user?.Company?.Logo) : null,
            FirstName = user?.FirstName??"کاربر",
            LastName = user?.LastName??"مهمان",
            Id = user?.Id,
            NationalCode = user?.NationalCode,
            IDPId = user?.IDPId,
            PhoneNumber = user?.PhoneNumber,
            UserRoles = user?.UserRoles?.ToDictionary(p => p.RoleType, p => ((Enums.UserRoleType)p.RoleType).ToString()),
            UserRolesList = user?.UserRoles?.Select(u => u.RoleType).ToList(),
        };
        return Result<UserViewModel>.SuccessResult(userViewModel);
    }
}