using CPG.Domain.SharedKernel.Minio;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Application.Shared.Exceptions;
using CPG.Domain.AggregateModels.ApplicationAggregate.Specifications;

namespace CPG.Application.UseCases.Applications.Commands.CreateApplication;

internal class CreateApplicationCommandHandler(IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> applicationRepository, IMinioProvider minioProvider) 
    : IRequestHandler<CreateApplicationCommand, long>
{
    private readonly IAggregateRepository<Domain.AggregateModels.ApplicationAggregate.Application> _applicationRepository = applicationRepository;
    private readonly IMinioProvider _minioProvider = minioProvider;

    public async Task<long> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var samePersianNameApplication = await _applicationRepository.GetBySpecAsync(new ApplicationByPersianName(request.Model.PersianName), cancellationToken);
        if (samePersianNameApplication != null)
        {
            throw new DuplicatePersianNameException(request.Model.PersianName);
        }
        var sameEnglishNameApplication = await _applicationRepository.GetBySpecAsync(new ApplicationByEnglishName(request.Model.EnglishName), cancellationToken);
        if (sameEnglishNameApplication != null)
        {
            throw new DuplicateEnglishNameException(request.Model.EnglishName);
        }
        PersianName persianName = new(request.Model.PersianName);
        EnglishName englishName = new(request.Model.EnglishName);
        Logo logo = new(request.Model.File, Enums.UploadFromEntityType.Application.ToString(), _minioProvider);

        var application = Domain.AggregateModels.ApplicationAggregate.Application.Create(persianName, englishName, request.Model.ResponseApiUrl, logo, request.Model.IdpClientIds);

        await _applicationRepository.AddAsync(application, cancellationToken);
        await _applicationRepository.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}