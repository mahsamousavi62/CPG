using CPG.Application.UseCases.PaymentRequests.Queries;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentRequests;

public class GetPaymentRequestQueryHandler(ReadDbContext context) : IRequestHandler<GetPaymentRequestQuery, Result<IReadOnlyCollection<PaymentRequestViewModel>>>
{
    private readonly ReadDbContext _context = context;

    public async Task<Result<IReadOnlyCollection<PaymentRequestViewModel>>> Handle(GetPaymentRequestQuery request, CancellationToken cancellationToken)
    {
        var paymentRequests = await _context.PaymentRequestReadModels.Include(p => p.Application)
            .Include(p => p.Company).ToListAsync(cancellationToken);

        var viewModels = paymentRequests.Select(p => new PaymentRequestViewModel
        {
            ApplicationId = p.ApplicationId,
            Amount = p.Amount,
            ApplicationName = p.Application.PersianName,
            CallBackUrl = p.CallBackUrl,
            PaymentCode = p.PaymentCode,
            CompanyId = p.CompanyId,
            CompanyName = p.Company.PersianName,
            CreationDate = p.CreationDate,
            Description = p.Description,
            DestinationIban = p.DestinationDepositIban,
            Id = p.Id,
            IsActive = p.IsActive,
            IsUsed = p.IsUsed,
            IsAnonymous = p.IsAnonymous,
            ModificationDate = p.ModificationDate,
            NationalCode = p.NationalCode,
            Status = p.Status,
            TrackerId = p.TrackerId,
            UrlExpirationDateTime = p.UrlExpirationDateTime,
            VerificationDateTime = p.VerificationDateTime,
        }).ToList();

        return Result<IReadOnlyCollection<PaymentRequestViewModel>>.SuccessResult(viewModels);
    }
}
