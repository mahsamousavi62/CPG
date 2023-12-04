using CPG.Application.UseCases.Providers.Exceptions;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Providers.Commands.ActivateProvider;

public class ActivateProviderCommandHandler(IAggregateRepository<Provider> providerRepository, ICurrentUser currentUser) : IRequestHandler<ActivateProviderCommand>
{
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task Handle(ActivateProviderCommand command, CancellationToken cancellationToken)
    {
        var bank = await _providerRepository.GetByIdAsync(command.ProviderId, cancellationToken)
                   ?? throw new ProviderNotFoundException(command.ProviderId);

        if (command.IsActive)
            bank.SetAsActive(_currentUser.UserId);
        else
            bank.SetAsInactive(_currentUser.UserId);

        await _providerRepository.SaveChangesAsync(cancellationToken);
    }
}