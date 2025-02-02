using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Queries.AnonymousStatus;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Infrastructure.Persistence.QueryHandlers.PaymentRequests;

public class AnonymousStatusQueryHandler(ReadDbContext context) : IRequestHandler<AnonymousStatusQuery, Result<AnonymousStatusResponseViewModel>>
{
    private readonly ReadDbContext _context = context;

    public async Task<Result<AnonymousStatusResponseViewModel>> Handle(AnonymousStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Model.PaymentCode) && string.IsNullOrEmpty(request.Model.PaymentCode))
            {
                throw new RequiredCodeOrTrackIdException();
            }
            var paymentRequest = await _context.PaymentRequestReadModels.FirstOrDefaultAsync(t => t.PaymentCode == request.Model.PaymentCode, cancellationToken: cancellationToken);

            if (paymentRequest == null)
            {
                throw new PaymentRequestNotFoundException(request.Model.PaymentCode);
            }

            return Result<AnonymousStatusResponseViewModel>.SuccessResult(new AnonymousStatusResponseViewModel { Status = paymentRequest.IsAnonymous });
        }
        catch (Exception ex)
        {
            return Result<AnonymousStatusResponseViewModel>.Failure(new Error(ex.Source, ex.Message));
        }
    }
}