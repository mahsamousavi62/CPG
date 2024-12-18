using CPG.Application.UseCases.Application.Commands.ActivateApplication;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Applications.Commands.ActivateApplication;

public class ActivateApplicationCommandHandler(IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> ApplicationRepository) : IRequestHandler<ActivateApplicationCommand, Result<bool>>
{
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _ApplicationRepository = ApplicationRepository;

    public async Task<Result<bool>> Handle(ActivateApplicationCommand command, CancellationToken cancellationToken)
    {
        var bank = await _ApplicationRepository.GetByIdAsync(command.ApplicationId, cancellationToken)
                   ?? throw new ApplicationNotFoundException(command.ApplicationId);

        if (command.IsActive)
            bank.SetAsActive();
        else
            bank.SetAsInactive();

        await _ApplicationRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success();
    }
}