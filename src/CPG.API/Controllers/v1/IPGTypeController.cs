using CPG.Application.UseCases.IPGTypes.Queries;
using CPG.Application.UseCases.IPGTypes.ViewModels;
using CPG.Domain.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1;

public class IPGTypeController : ApiBaseController
{
    [HttpGet("{id:int}")]
    public async Task<Result<IPGTypeViewModel>> GetIPGType(long id)
    { 
        return await Mediator.Send(new GetIPGTypeQuery(id));
    }

    [HttpGet("all")]
    public async Task<Result<IReadOnlyCollection<IPGTypeViewModel>>> GetAllIPGTypes()
    { 
        return await Mediator.Send(new GetAllIPGTypesQuery());
    }

    [HttpGet("active")]
    public async Task<Result<IReadOnlyCollection<IPGTypeViewModel>>> GetActiveIPGTypes()
    {
        return await Mediator.Send(new GetActiveIPGTypesQuery());
    }
}