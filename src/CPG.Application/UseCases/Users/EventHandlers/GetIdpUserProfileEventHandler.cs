using CPG.Application.Shared;
using CPG.Application.UseCases.Users.ViewModel;
using CPG.Domain.AggregateModels.UserAggregate.Events;
using MediatR;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.Users.EventHandlers;

public class GetIdpUserProfileEventHandler(IHttpClientFactoryService httpClientFactoryService) : INotificationHandler<GetIdpUserProfileEvent>
{
    private readonly IHttpClientFactoryService _httpClientFactoryService = httpClientFactoryService;

    public async Task Handle(GetIdpUserProfileEvent getIdpUserProfileEvent, CancellationToken cancellationToken)
    {
        var strModel = await _httpClientFactoryService.Execute(getIdpUserProfileEvent.IdpUserProfileModel);
        var idpUserProfile = JsonConvert.DeserializeObject<IdpUserProfile>(strModel);
    }
}
