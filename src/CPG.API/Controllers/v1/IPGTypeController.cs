using CPG.Application.UseCases.IPGType.Queries;
using CPG.Application.UseCases.IPGType.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    public class IPGTypeController : ApiBaseController
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IPGTypeViewModel>> GetIPGType(long id)
            => Ok(await Mediator.Send(new GetIPGTypeQuery(id)));

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyCollection<IPGTypeViewModel>>> GetAllIPGTypes()
            => Ok(await Mediator.Send(new GetAllIPGTypesQuery()));

        [HttpGet("active")]
        public async Task<ActionResult<IReadOnlyCollection<IPGTypeViewModel>>> GetActiveIPGTypes()
            => Ok(await Mediator.Send(new GetActiveIPGTypesQuery()));

    }
}
