using CPG.Application.UseCases.Application.Commands.ActivateApplication;
using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

/// <summary>
/// Client Application API
/// </summary>
public class ApplicationController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<Result<ApplicationViewModel>> GetApplication(long id)
    { 
        return await Mediator.Send(new GetApplicationQuery(id));
    }

    [HttpGet("all")]
    public async Task<Result<IReadOnlyCollection<ApplicationViewModel>>> GetAllApplications()
    { 
        return await Mediator.Send(new GetAllApplicationsQuery());
    }

    [HttpGet("active")]
    public async Task<Result<IReadOnlyCollection<ApplicationViewModel>>> GetActiveApplications()
    { 
        return await Mediator.Send(new GetActiveApplicationsQuery());
    }

    [HttpPost]
    public async Task<Result<long>> CreateApplication([FromForm] CreateApplicationModel model)
    {
        CreateApplicationViewModel createApplicationViewModel = new(model.PersianName, model.EnglishName, model.ResponseApiUrl, model.IdpClientIds, model.CallbackUrls, new FormFileProxy(model.File));

        return await Mediator.Send(new CreateApplicationCommand(createApplicationViewModel));        
    }

    [HttpPost("activate/{ApplicationId:long}/{isActive:bool}")]
    public async Task<IActionResult> ActivateApplication(int ApplicationId, bool isActive)
    {
        await Mediator.Send(new ActivateApplicationCommand(ApplicationId, isActive));

        return Accepted();
    }
}