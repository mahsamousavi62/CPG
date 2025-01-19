using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommonController : ApiBaseController
{
    protected readonly IResourceHelper resourceHelper = new ResourceHelper();

    [HttpGet("resources")]
    [AllowAnonymous]
    public Result<Dictionary<string, string>> GetResources()
    {
        return resourceHelper.GetResources();
    }

    [HttpGet("GetAppSetting/{entityType:int}")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<ApplicationSettingViewModel>>), (int)HttpStatusCode.OK)]
    [AllowAnonymous]
    public async Task<Result<IReadOnlyCollection<ApplicationSettingViewModel>>> GetAppSetting(int entityType)
    {
        var entityTypeEnum = (Enums.ApplicationSettingEntityType)entityType;
        return await Mediator.Send(new GetApplicationSettingsQuery(entityTypeEnum));
    }
}