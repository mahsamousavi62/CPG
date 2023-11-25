using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events;

public class GetIdpUserProfileEvent(GetIdpUserProfileModel idpUserProfileModel) : INotification
{
    public GetIdpUserProfileModel IdpUserProfileModel { get; set; } = idpUserProfileModel;
}
