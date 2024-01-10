using CPG.Application.UseCases.Exceptions;
using CPG.Domain.Exceptions;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users.Commands;

public class CreateRedirectUrlCommandHandler(IApplicationSettingsRepository applicationSettingsRepository) : IRequestHandler<CreateRedirectUrlCommnad, Result<string>>
{
    private readonly IApplicationSettingsRepository _applicationSettingsRepository = applicationSettingsRepository;

    public async Task<Result<string>> Handle(CreateRedirectUrlCommnad command, CancellationToken cancellationToken)
    {
        try
        {
            var appConfig = await _applicationSettingsRepository.GetAllApplicationSettings();

            return Result<string>.SuccessResult($"{appConfig.IPG_Callback_URL}?trackId={command.id}");
        }
        catch (DomainException exc)
        {
            return Result<string>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (AppException exc)
        {
            return Result<string>.Failure(new Error((exc as dynamic).Code, exc.Message));
        }
        catch (Exception exc)
        {
            return Result<string>.Failure(new Error(exc.Source, exc.Message));
        }
    }
}
