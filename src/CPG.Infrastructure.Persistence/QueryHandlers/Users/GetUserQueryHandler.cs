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
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users;

public class GetUserQueryHandler(ReadDbContext context, IAuthenticationService authenticationService,
    IMinioProvider minioProvider, IIdpProvider idpClient) 
    : IRequestHandler<GetUserQuery, Result<UserViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly string userKycStatus = "KycVerified";
    private readonly IMinioProvider _minioProvider = minioProvider;
    private readonly IIdpProvider _idpClient = idpClient;

    public async Task<Result<UserViewModel>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
		
            var sub = await _authenticationService.GetDataFromClaim<string>("sub", string.Empty);
            var kycStatus = await _authenticationService.GetDataFromClaim<string>("status", string.Empty);
            if (kycStatus != userKycStatus)
            {
                var idpUserProfileResponse = await _idpClient.GetUserProfile(sub);

                if (idpUserProfileResponse.Data.StatusCode == (short)HttpStatusCode.NotFound ||
                  idpUserProfileResponse.OperationResult == Enums.OperationResult.Failed
                  || string.IsNullOrEmpty(idpUserProfileResponse.Data?.Result?.Id))
                    throw new UserNotVerifyStatusException(string.Empty);
            }

            var user = await _context.UserReadModels.Include(u => u.UserRoles).Include(c=>c.Company)
                .SingleOrDefaultAsync(u => u.IsActive && u.IDPId == sub);
            
            var applicationId = (await _context.ApplicationIdentifierReadModels.SingleOrDefaultAsync(a => a.IdpClientId == sub))?.ApplicationId;
            
            if (user == null)
                throw new UserNotFoundException(sub);

            var userViewModel = new UserViewModel
            {
                CompanyId=user.Company?.Id,
                CompanyPersianName = user.Company?.PersianName,
                CompanyLogo = !string.IsNullOrEmpty(user.Company?.Logo) ? await General.GetLogo(minioProvider, user.Company.Logo) : null,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                NationalCode = user.NationalCode,
                IDPId = user.IDPId,
                PhoneNumber = user.PhoneNumber,
                UserRoles = user.UserRoles.ToDictionary(p => p.RoleType,p => ((Enums.UserRoleType)p.RoleType).ToString()),
                UserRolesArray=user.UserRoles.Select(u=>u.RoleType).ToArray(),
            };
            return Result<UserViewModel>.SuccessResult(userViewModel);
      
    }
}


