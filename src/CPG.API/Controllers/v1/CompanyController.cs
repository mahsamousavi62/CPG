using CPG.Application.UseCases.Companies.Commands.CreateCompany;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
    [EnumDataType(typeof(Enums.CompanyPaymentMethodType))]
    public async Task<IActionResult> GetCompanyPaymentMethodsType(Enums.CompanyPaymentMethodType type)
        => Ok(await Mediator.Send(new GetCompanyPaymentMethodsQuery()));

    [HttpGet("GetIpgRedirectionMethodType")]
    [EnumDataType(typeof(Enums.IpgRedirectionMethodType))]
    public async Task<IActionResult> GetIpgRedirectionMethodType(Enums.IpgRedirectionMethodType type)
       => Ok(await Mediator.Send(new GetIpgRedirectionMethodTypeQuery()));


    [HttpPost]
    public async Task<Result<long>> CreateCompany([FromForm] CreateCompanyModel model)
    {
        CreateCompanyViewModel createCompanyViewModel = new(
             model.PersianName, model.EnglishName, model.NationalCodeMatchingRequied,
             new FormFileProxy(model.File), model.MethodTypes, model.Users,model.SiteAddress,model.IpgRedirectionMethodType);

        var result = await Mediator.Send(new CreateCompanyCommand(createCompanyViewModel));

        return result;
    }
}
