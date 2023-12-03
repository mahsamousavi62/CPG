using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Specifications;
using CPG.Application.UseCases.Providers.Exceptions;

namespace CPG.Application.UseCases.Providers.Commands.CreateProvider;

internal class CreateProviderCommandHandler(IAggregateRepository<Provider> providerRepository, IMinioProvider minioProvider) 
    : IRequestHandler<CreateProviderCommand, long>
{
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<long> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        var samePersianNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByPersianName(request.Model.PersianName));
        if (samePersianNameProvider != null)
        {
            throw new DuplicateProviderPersianNameException(request.Model.PersianName);
        }
        var sameEnglishNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByEnglishName(request.Model.EnglishName));
        if (sameEnglishNameProvider != null)
        {
            throw new DuplicateProviderEnglishNameException(request.Model.EnglishName);
        }
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Provider.ToString(), _minioProvider);

        var provider = Provider.Create(persianName, englishName, request.Model.ProviderType, logo, request.Model.ProviderData);

        await _providerRepository.AddAsync(provider, cancellationToken);
        await _providerRepository.SaveChangesAsync(cancellationToken);

        return provider.Id;
    }
}