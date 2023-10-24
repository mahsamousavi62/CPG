using CPG.Application.Shared.Resource;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {
        protected readonly IResourceHelper resourceHelper = new ResourceHelper();

        [HttpGet("resources")]
        [AllowAnonymous]
        public ActionResult<Dictionary<string, string>> GetResources()
        {
            return Ok(resourceHelper.GetResources());
        }
    }
}
