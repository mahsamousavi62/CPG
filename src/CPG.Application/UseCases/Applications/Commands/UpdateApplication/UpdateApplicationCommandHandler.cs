using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CPG.Application.Shared.Exceptions;
using CPG.Application.UseCases.Application.Exceptions;
using CPG.Application.UseCases.Applications.ViewModels;
using CPG.Application.UseCases.CompanyDeposits;
using CPG.Application.UseCases.CompanyDeposits.Exceptions;
using CPG.Application.UseCases.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Minio;
using MediatR;

namespace CPG.Application.UseCases.Applications.Commands.UpdateApplication;

public class UpdateApplicationCommandHandlerr(IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository,
                                               IReadRepository<ApplicationIdentifier> identifierRepository,
                                               IMinioProvider minioProvider) : IRequestHandler<UpdateApplicationCommand, Result<Unit>>
{
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;
    private readonly IReadRepository<ApplicationIdentifier> _identifierRepository = identifierRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<Unit>> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {

        try
        {
            var(application, persianName, englishName) = await Validate(request.Model);

           Url responseUrl = new(request.Model.ResponseApiUrl);
            var urls = request.Model.CallbackUrls?.Select(t => new Url(t)).ToArray();

            Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Application.ToString(), _minioProvider);
             Domain.AggregateModels.ApplicationAggregate.Application.Update(application, persianName, englishName, responseUrl, 
                                                                                             logo, request.Model.IdpClientIds, urls);

            await _applicationRepository.UpdateAsync(application, cancellationToken);
            await _applicationRepository.SaveChangesAsync(cancellationToken);

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

    private async Task<(Domain.AggregateModels.ApplicationAggregate.Application application, PersianName persianName, EnglishName englishName)> Validate(UpdateApplicationViewModel model)
    {
        PersianName persianName = new(model.PersianName);
        EnglishName englishName = new(model.EnglishName);

        var application = await _applicationRepository.GetBySpecAsync(new ApplicationByIdSpec(model.Id));
        if (application == null)
            throw new ApplicationNotFoundException(model.Id);

        var samePersianNameApplication = await _applicationRepository.GetBySpecAsync(new ApplicationByPersianNameUpdateMode(model.PersianName,model.Id));
        if (samePersianNameApplication != null)
            throw new DuplicatePersianNameException(model.PersianName);

        var sameEnglishNameApplication = await _applicationRepository.GetBySpecAsync(new ApplicationByEnglishNameUpdateMode(model.EnglishName,model.Id));
        if (sameEnglishNameApplication != null)
            throw new DuplicateEnglishNameException(model.EnglishName);

        var applicationByClinetIds = await _applicationRepository.GetBySpecAsync(new ApplicationbyIdpClientIdUpdateMode(model.IdpClientIds, model.Id));
        if (applicationByClinetIds is not null && (applicationByClinetIds.ApplicationIdentifiers != null || applicationByClinetIds.ApplicationIdentifiers.Count != 0))
            throw new DuplicateIdpClientIdsException(string.Join('-', applicationByClinetIds.ApplicationIdentifiers.Select(t => t.IdpClientId)));

        if (model.CallbackUrls != null)
        {
            var applicationByCallBackUrls = await _applicationRepository.GetBySpecAsync(new ApplicationByCallBackUrlUpdateMode(model.CallbackUrls, model.Id));
            if (applicationByCallBackUrls is not null && (applicationByCallBackUrls.ApplicationCallbackUrls != null
                || applicationByCallBackUrls.ApplicationCallbackUrls.Count != 0))
                throw new DuplicateCallbackUrlException(string.Join('-', applicationByCallBackUrls.ApplicationCallbackUrls.Select(t => t.CallbackUrl)));
        }
        return (application, persianName, englishName);
    }
}
