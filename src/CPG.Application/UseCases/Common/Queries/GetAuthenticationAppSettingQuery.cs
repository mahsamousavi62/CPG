using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.ApplicationSettings;
using MediatR;

namespace CPG.Application.UseCases.Common.Queries
{
    public class GetAuthenticationAppSettingQuery:IRequest<ApplicationConfigViewModel>
    {
        public Enums.ApplicationSettingEntityType EntityType { get; } = Enums.ApplicationSettingEntityType.IDPCredential;
    }
}
