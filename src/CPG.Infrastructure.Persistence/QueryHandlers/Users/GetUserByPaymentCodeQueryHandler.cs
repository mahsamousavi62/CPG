
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.Users.Exceptions;
using CPG.Application.UseCases.Users.Queries;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.Users;

public class GetUserByPaymentCodeQueryHandler(ReadDbContext context, IMinioProvider minioProvider) 
    : IRequestHandler<GetUserByPaymentCodeQuery, Result<UserViewModel>>
{
    private readonly ReadDbContext _context = context;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<UserViewModel>> Handle(GetUserByPaymentCodeQuery request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _context.PaymentRequestReadModels.FirstOrDefaultAsync(c => c.PaymentCode == request.PaymentCode);
        if (paymentRequest is null) throw new PaymentRequestNotFoundByCodeException();


        var nationalCode = paymentRequest.NationalCode;
        var user = await _context.UserReadModels.Include(u => u.UserRoles).Include(c => c.Company)
        .SingleOrDefaultAsync(u => u.IsActive && u.NationalCode == nationalCode);

        if (user != null)
        {
            var userViewModel = new UserViewModel
            {
                CompanyId = user.Company?.Id,
                CompanyPersianName = user.Company?.PersianName,
                CompanyLogo = !string.IsNullOrEmpty(user.Company?.Logo) ? await General.GetLogo(_minioProvider, user.Company.Logo) : null,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                NationalCode = user.NationalCode,
                IDPId = user.IDPId,
                PhoneNumber = user.PhoneNumber,
                UserRoles = user.UserRoles.ToDictionary(p => p.RoleType, p => ((Enums.UserRoleType)p.RoleType).ToString()),
                UserRolesList = user.UserRoles.Select(u => u.RoleType).ToList(),
            };
            return Result<UserViewModel>.SuccessResult(userViewModel);
        }
        return Result<UserViewModel>.SuccessResult(null);
    }
}
