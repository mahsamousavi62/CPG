using CPG.Domain.SharedKernel.Minio;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Application.Shared.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;
using CPG.Domain.AggregateModels.ApplicationAggregate;
using Ardalis.Specification;
using CPG.Domain.AggregateModels.ApplicationAggregate.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System;
using CPG.Domain.SharedKernel.Interfaces;

namespace CPG.Application.UseCases.Applications.Commands.CreateApplication;

internal class CreateApplicationCommandHandler(IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository,
                                               IReadRepository<ApplicationIdentifier> identifierRepository,
                                               IMinioProvider minioProvider)
    : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;
    private readonly IReadRepository<ApplicationIdentifier> _identifierRepository = identifierRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);

        var samePersianNameApplication = await _applicationRepository.FirstOrDefaultAsync(new ApplicationByPersianName(request.Model.PersianName), cancellationToken);
        if (samePersianNameApplication != null)
        {
            throw new DuplicatePersianNameException(request.Model.PersianName);
        }
        var sameEnglishNameApplication = await _applicationRepository.FirstOrDefaultAsync(new ApplicationByEnglishName(request.Model.EnglishName), cancellationToken);
        if (sameEnglishNameApplication != null)
        {
            throw new DuplicateEnglishNameException(request.Model.EnglishName);
        }

        var  applicationByClinetIds = await _applicationRepository.FirstOrDefaultAsync(new ApplicationbyIdpClientId(request.Model.IdpClientIds));
                
        if (applicationByClinetIds is not null &&(applicationByClinetIds.ApplicationIdentifiers != null || applicationByClinetIds.ApplicationIdentifiers.Count != 0))
        {
            throw new DuplicateIdpClientIdsException(string.Join('-', applicationByClinetIds.ApplicationIdentifiers.Select(t => t.IdpClientId)));            
        }

        if (request.Model.CallbackUrls!=null)
        {
            var applicationByCallBackUrls = await _applicationRepository.FirstOrDefaultAsync(new ApplicationByCallBackUrl(request.Model.CallbackUrls));
            if (applicationByCallBackUrls is not null && (applicationByCallBackUrls.ApplicationCallbackUrls != null
                || applicationByCallBackUrls.ApplicationCallbackUrls.Count != 0))
            {
                throw new DuplicateCallbackUrlException(string.Join('-', applicationByCallBackUrls.ApplicationCallbackUrls.Select(t => t.CallbackUrl)));
            } 
        }

        Url responseUrl = new(request.Model.ResponseApiUrl);
        var urls = request.Model.CallbackUrls?.Select(t => new Url(t)).ToArray();

        Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Application.ToString(), _minioProvider);
        var application = Domain.AggregateModels.ApplicationAggregate.Application.Create(persianName, englishName, responseUrl, logo, request.Model.IdpClientIds, urls);

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return Result<long>.SuccessResult(application.Id);
    }
}