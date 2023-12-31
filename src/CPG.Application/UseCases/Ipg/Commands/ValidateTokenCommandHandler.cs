using CPG.Application.UseCases.Ipg.Exceptions;
using CPG.Domain.AggregateModels.TransactionAggregate;
using CPG.Domain.AggregateModels.TransactionAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.Ipg.Commands;

public class ValidateTokenCommandHandler(IAggregateRepository<Transaction> repository) : IRequestHandler<ValidateTokenCommand>
{    
    private readonly IAggregateRepository<Transaction> _repository = repository;

    public async Task Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetBySpecAsync(new TransactionByIPGTrackId(command.ValidateToken.TrackId));

        if (transaction?.IPGTransaction is null)
            throw new NotFoundTrackIdException();
        if(transaction.IPGTransaction.Status != IPGTransactionStatus.WaitingForPspResponse)
            throw new TrackIdInvalidStatusException();
    }
}