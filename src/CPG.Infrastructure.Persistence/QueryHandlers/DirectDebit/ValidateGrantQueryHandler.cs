using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.SharedKernel.Communication.DirectDebit;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate;
using CPG.Application.UseCases.DirectDebit.Queries;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.AggregateModels.DirectDebitGrantAggregate.Specifications;
using CPG.Application.UseCases.DirectDebit.Exceptions;

namespace CPG.Infrastructure.Persistence.QueryHandlers.DirectDebit;

public class ValidateGrantQueryHandler(IDirectDebitFactory directDebitFactory,
    IAggregateRepository<DirectDebitGrant> directDebitGrantRepository,
    IAggregateRepository<Transaction> transactionRepository) : IRequestHandler<ValidateGrantQuery, Result<ValidateGrantResponseViewModel>>
{   
    private readonly IDirectDebitFactory directDebitFactory = directDebitFactory;
    private readonly IAggregateRepository<DirectDebitGrant> _directDebitGrantRepository = directDebitGrantRepository;
    private readonly IAggregateRepository<Transaction> _transactionRepository = transactionRepository;
   
    public async Task<Result<ValidateGrantResponseViewModel>> Handle(ValidateGrantQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var directDebitGrant = await _directDebitGrantRepository.GetBySpecAsync(new DirectDebitGrantByTrackIdSpec(request.TrackId), cancellationToken);
            
            if (directDebitGrant is null)
                throw new NotFoundTrackIdException();
            if (directDebitGrant.Status != DirectDebitGrantStatus.Draft)
                throw new TrackIdInvalidStatusException();
            
            return Result<ValidateGrantResponseViewModel>.SuccessResult(new ValidateGrantResponseViewModel
            {
                
            });
        }
        catch (DomainException exc)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception)
        {
            return Result<ValidateGrantResponseViewModel>.Failure(new Error("1010000", GlobalResource.UnexpectedError));
        }
    }
}
