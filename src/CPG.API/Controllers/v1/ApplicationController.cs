using AuthDemo.Security.Authorization;
using CPG.Application.UseCases.Application.Commands.ActivateApplication;
using CPG.Application.UseCases.Application.Commands.CreateApplication;
using CPG.Application.UseCases.Application.Queries;
using CPG.Application.UseCases.Application.ViewModels;
using CPG.Application.UseCases.Applications.Commands.UpdateApplication;
using CPG.Application.UseCases.Applications.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost]
    public async Task<Result<long>> CreateApplication([FromForm] CreateApplicationModel model)
    {
        CreateApplicationViewModel createApplicationViewModel = new(model.PersianName, model.EnglishName, model.ResponseApiUrl, model.IdpClientIds, model.CallbackUrls, new FormFileProxy(model.File));

        return await Mediator.Send(new CreateApplicationCommand(createApplicationViewModel));        
    }
    
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPut]
    public async Task<Result<Unit>> UpdateApplication([FromForm] UpdateApplicationModel model)
    {
        UpdateApplicationViewModel createApplicationViewModel = new(model.Id, model.PersianName, model.EnglishName, model.ResponseApiUrl, model.IdpClientIds, model.CallbackUrls, new FormFileProxy(model.File));

        return await Mediator.Send(new UpdateApplicationCommand(createApplicationViewModel));
    }
   
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost("activate/{ApplicationId:long}/{isActive:bool}")]
    public async Task<Result<bool>> ActivateApplication(int ApplicationId, bool isActive)
    {
        return await Mediator.Send(new ActivateApplicationCommand(ApplicationId, isActive));
    }
}