using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Specifications;
using CPG.Application.UseCases.Providers.Exceptions;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Application.UseCases.Providers.Commands.CreateProvider;

internal class CreateProviderCommandHandler(IAggregateRepository<Provider> providerRepository, IMinioProvider minioProvider) 
    : IRequestHandler<CreateProviderCommand, Result<long>>
{
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<long>> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        
        var samePersianNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByPersianName(request.Model.PersianName));
        if (samePersianNameProvider != null)
        {
            throw new DuplicateProviderPersianNameException(request.Model.PersianName);
        }
        var sameEnglishNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByEnglishName(request.Model.EnglishName));
        if (sameEnglishNameProvider != null)
            throw new DuplicateProviderEnglishNameException(request.Model.EnglishName);


        var sameProvidertype = await _providerRepository.GetBySpecAsync(new ProviderByProviderType(request.Model.ProviderType));
        if (sameProvidertype!=null)
            throw new DuplicateProviderTypeException(request.Model.ProviderType);
        
        Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Provider.ToString(), _minioProvider);

        var provider = Provider.Create(persianName, englishName, request.Model.ProviderType,logo, request.Model.ProviderData, request.Model.MethodTypes);

        await _providerRepository.AddAsync(provider, cancellationToken);
        await _providerRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.SuccessResult(provider.Id);
    }
}