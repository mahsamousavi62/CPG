using CPG.Application.UseCases.Providers.Commands.ActivateProvider;
using CPG.Application.UseCases.Providers.Commands.CreateProvider;
using CPG.Application.UseCases.Providers.Commands.UpdateProvider;
using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

public class ProviderController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<Result<ProviderViewModel>> GetProvider(long id)
    {
        return await Mediator.Send(new GetProviderQuery(id));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpGet("all")]
    public async Task<Result<IReadOnlyCollection<ProviderViewModel>>> GetAllProviders()
    {
        return await Mediator.Send(new GetAllProvidersQuery());
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpGet("active")]
    public async Task<Result<IReadOnlyCollection<ProviderViewModel>>> GetActiveProviders()
    {
        return await Mediator.Send(new GetActiveProvidersQuery());
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost]
    public async Task<Result<long>> CreateProvider([FromForm] CreateProviderModel model)
    {
        CreateProviderViewModel createProviderViewModel = new(model.PersianName, model.EnglishName,
                                                             model.ProviderType, model.ProviderData,
                                                             new FormFileProxy(model.File), model.MethodTypes);

        return await Mediator.Send(new CreateProviderCommand(createProviderViewModel));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPut]
    public async Task<Result<Unit>> UpdateProvider([FromForm] UpdateProviderModel model)
    {
        UpdateProviderViewModel updateProviderViewModel = new(model.Id, model.PersianName, model.EnglishName,
                                                              model.ProviderData, new FormFileProxy(model.File),
                                                              model.MethodTypes);

        return await Mediator.Send(new UpdateProviderCommand(updateProviderViewModel));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost("activate/{providerId:long}/{isActive:bool}")]
    public async Task<Result<bool>> ActivateProvider(int providerId, bool isActive)
    {
        return await Mediator.Send(new ActivateProviderCommand(providerId, isActive));
    }
}