using CPG.API.Model;
using CPG.Application.UseCases.Providers.Commands.ActivateProvider;
using CPG.Application.UseCases.Providers.Commands.CreateProvider;
using CPG.Application.UseCases.Providers.Queries;
using CPG.Application.UseCases.Providers.ViewModels;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

public class ProviderController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProviderViewModel>> GetProvider(long id)
        => Ok(await Mediator.Send(new GetProviderQuery(id)));

    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyCollection<ProviderViewModel>>> GetAllProviders()
        => Ok(await Mediator.Send(new GetAllProvidersQuery()));

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyCollection<ProviderViewModel>>> GetActiveProviders()
        => Ok(await Mediator.Send(new GetActiveProvidersQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateProvider([FromForm] CreateProviderModel model)
    {
        CreateProviderViewModel createProviderViewModel = new(model.PersianName, model.EnglishName,
                                                             model.ProviderType,model.ProviderData,
                                                             new FormFileProxy(model.File));

        var providerId = await Mediator.Send(new CreateProviderCommand(createProviderViewModel));

        return CreatedAtAction(nameof(CreateProvider), new { id = providerId }, new { providerId });
    }

    [HttpPost("activate/{providerId:long}/{isActive:bool}")]
    public async Task<IActionResult> ActivateProvider(int providerId, bool isActive)
    {
        await Mediator.Send(new ActivateProviderCommand(providerId, isActive));

        return Accepted();
    }
}