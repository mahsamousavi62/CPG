using CPG.Application.Shared;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using CPG.Domain.AggregateModels.UserAggregate.UserViewModel;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users.EventHandlers
{
    public class GetIdpUserProfileEventHandler : INotificationHandler<GetIdpUserProfileEvent>
    {
        private readonly IHttpClientFactoryService _httpClientFactoryService;

        public GetIdpUserProfileEventHandler(IHttpClientFactoryService httpClientFactoryService)
        {
             _httpClientFactoryService = httpClientFactoryService;
        }
        public async Task Handle(GetIdpUserProfileEvent getIdpUserProfileEvent, CancellationToken cancellationToken)
        {
            var strModel = await _httpClientFactoryService.Execute(getIdpUserProfileEvent.IdpUserProfileModel);
            var idpUserProfile = JsonConvert.DeserializeObject<IdpUserProfile>(strModel);
        }
    }
}
