using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers;

/// <summary>
/// Abstract Class for Api's
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiBaseController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
