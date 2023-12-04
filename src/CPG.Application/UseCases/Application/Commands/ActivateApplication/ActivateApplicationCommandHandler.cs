using CPG.Application.UseCases.Application.Commands.ActivateApplication;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Applications.Commands.ActivateApplication;

public class ActivateApplicationCommandHandler(IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> ApplicationRepository, ICurrentUser currentUser) : IRequestHandler<ActivateApplicationCommand>
{
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _ApplicationRepository = ApplicationRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(ActivateApplicationCommand command, CancellationToken cancellationToken)
    {
        var bank = await _ApplicationRepository.GetByIdAsync(command.ApplicationId, cancellationToken)
                   ?? throw new ApplicationNotFoundException(command.ApplicationId);

        if (command.IsActive)
            bank.SetAsActive(_currentUser.UserId);
        else
            bank.SetAsInactive(_currentUser.UserId);

        await _ApplicationRepository.SaveChangesAsync(cancellationToken);
    }
}