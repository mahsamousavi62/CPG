using CPG.Application.UseCases.Application.Commands.ActivateApplication;
using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

/// <summary>
/// Client Application API
/// </summary>
public class ApplicationController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApplicationViewModel>> GetApplication(long id)
        => Ok(await Mediator.Send(new GetApplicationQuery(id)));

    [HttpGet("all")]
    public async Task<ActionResult<IReadOnlyCollection<ApplicationViewModel>>> GetAllApplications()
        => Ok(await Mediator.Send(new GetAllApplicationsQuery()));

    [HttpGet("active")]
    public async Task<ActionResult<IReadOnlyCollection<ApplicationViewModel>>> GetActiveApplications()
        => Ok(await Mediator.Send(new GetActiveApplicationsQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateApplication([FromForm] CreateApplicationModel model)
    {
        CreateApplicationViewModel createApplicationViewModel = new(model.PersianName, model.EnglishName, model.ResponseApiUrl, model.IdpClientIds, new FormFileProxy(model.File));

        var ApplicationId = await Mediator.Send(new CreateApplicationCommand(createApplicationViewModel));

        return CreatedAtAction(nameof(CreateApplication), new { id = ApplicationId }, new { ApplicationId });
    }

    [HttpPost("activate/{ApplicationId:long}/{isActive:bool}")]
    public async Task<IActionResult> ActivateApplication(int ApplicationId, bool isActive)
    {
        await Mediator.Send(new ActivateApplicationCommand(ApplicationId, isActive));

        return Accepted();
    }
}