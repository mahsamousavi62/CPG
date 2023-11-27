using CPG.API.Model;
using CPG.Application.UseCases.Companies.Commands.CreateCompany;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers;

public class CompanyController : ApiBaseController
{
    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CompanyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<CompanyViewModel>>> GetAll()
     => Ok(await Mediator.Send(new GetAllCompanyQuery()));

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CompanyViewModel), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CompanyViewModel>> GetCompany(long id)
        => Ok(await Mediator.Send(new GetCompanyQuery(id)));

    [HttpGet("GetCompanyPaymentMethodsType")]
    [ProducesResponseType(typeof(Dictionary<int, string>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetCompanyPaymentMethodsType()
        => Ok(await Mediator.Send(new GetCompanyPaymentMethodsQuery()));

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyModel model)
    {
        CreateCompanyViewModel createCompanyViewModel = new(
             model.PersianName, model.EnglishName, model.NationalCodeMatchingRequied,
             new FormFileProxy(model.File), model.MethodTypes, model.Users);

        var companyId = await Mediator.Send(new CreateCompanyCommand(createCompanyViewModel));

        return CreatedAtAction(nameof(CreateCompany), new { id = companyId }, new { companyId });
    }
}
