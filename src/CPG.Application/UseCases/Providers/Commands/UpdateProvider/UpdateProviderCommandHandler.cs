using System;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.Shared.Exceptions;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Application.UseCases.Applications.ViewModels;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.Providers.Exceptions;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.ProviderAggregate;
using CPG.Domain.AggregateModels.ProviderAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;

namespace CPG.Application.UseCases.Providers.Commands.UpdateProvider;

public class UpdateProviderCommandHandler(IAggregateRepository<Provider> providerRepository, IMinioProvider minioProvider) : IRequestHandler<UpdateProviderCommand, Result<Unit>>
{
    private readonly IAggregateRepository<Provider> _providerRepository = providerRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<Unit>> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (provider, persianName, englishName) = await Validate(request.Model);
            Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Provider.ToString(), _minioProvider);

             Provider.Update(provider, persianName, englishName, request.Model.ProviderType, logo, request.Model.ProviderData, request.Model.MethodTypes);

            await _providerRepository.UpdateAsync(provider, cancellationToken);
            await _providerRepository.SaveChangesAsync(cancellationToken);

            return Result<Unit>.SuccessResult(Unit.Value);
        }
        catch (DomainException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<Unit>.Failure(new Error(exc.Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<Unit>.Failure(new Error(exc.Source, exc.Message));
        }

    }

    private async Task<(Provider provider, PersianName persianName, EnglishName englishName)> Validate(UpdateProviderViewModel model)
    {
        PersianName persianName = new(model.PersianName);
        EnglishName englishName = new(model.EnglishName);

        var provider = await _providerRepository.GetByIdAsync(model.Id);
        if (provider == null)
            throw new ProviderNotFoundException(model.Id);

        var samePersianNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByPersianName(model.PersianName));
        if (samePersianNameProvider != null)
        {
            throw new DuplicateProviderPersianNameException(model.PersianName);
        }
        var sameEnglishNameProvider = await _providerRepository.GetBySpecAsync(new ProviderByEnglishName(model.EnglishName));
        if (sameEnglishNameProvider != null)
            throw new DuplicateProviderEnglishNameException(model.EnglishName);


        var sameProvidertype = await _providerRepository.GetBySpecAsync(new ProviderByProviderType(model.ProviderType));
        if (sameProvidertype != null)
            throw new DuplicateProviderTypeException(model.ProviderType);
    
        return (provider, persianName, englishName);

    }
}
