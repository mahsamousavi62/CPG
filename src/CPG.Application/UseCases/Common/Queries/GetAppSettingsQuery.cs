
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Common.Queries;

public class GetApplicationSettingsQuery : IRequest<IReadOnlyCollection<ApplicationSettingViewModel>>
{
    public Enums.ApplicationSettingEntityType EntityType { get; }

    public GetApplicationSettingsQuery(Enums.ApplicationSettingEntityType entityType) => EntityType = entityType;
}
