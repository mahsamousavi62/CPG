using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using MediatR;

namespace CPG.Domain.AggregateModels.UserAggregate.Events
{
    public class GetIdpUserProfileEvent:INotification
    {
        public GetIdpUserProfileModel  IdpUserProfileModel { get; set; }

        public GetIdpUserProfileEvent(GetIdpUserProfileModel idpUserProfileModel)
        {
            IdpUserProfileModel = idpUserProfileModel;
        }
    }
}
