using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ApiBaseController
    {
        protected readonly IResourceHelper resourceHelper = new ResourceHelper();

        [HttpGet("resources")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, string>> GetResources()
        {
            return Ok(resourceHelper.GetResources());
        }

        [HttpGet("GetAppSettingIDPCredential")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<ApplicationSettingViewModel>>> GetAppSettingIDPCredential()
        {
            return Ok(await Mediator.Send(new GetApplicationSettingsQuery(Enums.ApplicationSettingEntityType.IDPCredential)));
        }


        [HttpGet("GetAppSetting/{entityType:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<ApplicationSettingViewModel>>> GetAppSetting(int entityType)
        {
            var entityTypeEnum = (Enums.ApplicationSettingEntityType)entityType;
            return Ok(await Mediator.Send(new GetApplicationSettingsQuery(entityTypeEnum)));
        }

    }
}
