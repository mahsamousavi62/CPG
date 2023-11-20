using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
