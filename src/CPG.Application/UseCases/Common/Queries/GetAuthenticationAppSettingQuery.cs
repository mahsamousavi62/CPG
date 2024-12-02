using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using MediatR;

namespace CPG.Application.UseCases.Common.Queries
{
    public class GetAuthenticationAppSettingQuery: IRequest<JwtConfigViewModel>
    {
        public Enums.ApplicationSettingEntityType EntityType { get; } = Enums.ApplicationSettingEntityType.IDPCredential;
    }
}
