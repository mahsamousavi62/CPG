
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using MediatR;
using System.Collections.Generic;

namespace CPG.Application.UseCases.Common.Queries;

public class GetApplicationSettingsQuery(Enums.ApplicationSettingEntityType entityType) : IRequest<IReadOnlyCollection<ApplicationSettingViewModel>>
{
    public Enums.ApplicationSettingEntityType EntityType { get; } = entityType;
}
