using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Authorization;
using MediatR;

namespace CPG.Application.UseCases.Common.Queries;

public class GetAuthenticationAppSettingQuery:IRequest<AuthenticationConfigViewModel>
{
    public Enums.ApplicationSettingEntityType EntityType { get; } = Enums.ApplicationSettingEntityType.IDPCredential;
}
