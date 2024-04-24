using CPG.Application.UseCases.Companies.Commands.CreateCompany;
using CPG.Application.UseCases.Companies.Commands.UpdateCompany;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CPG.API.Controllers;

public class CompanyController : ApiBaseController
{
    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(Result<IReadOnlyCollection<CompanyViewModel>>), (int)HttpStatusCode.OK)]
    public async Task<Result<IReadOnlyCollection<CompanyViewModel>>> GetAll()
    {
        return await Mediator.Send(new GetAllCompanyQuery());
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(Result<CompanyViewModel>), (int)HttpStatusCode.OK)]
    public async Task<Result<CompanyViewModel>> GetCompany(long id)
    {
        return await Mediator.Send(new GetCompanyQuery(id));
    }

    [HttpGet("GetCompanyPaymentMethodsType")]
    [EnumDataType(typeof(Enums.PaymentMethodType))]
    public async Task<IActionResult> GetCompanyPaymentMethodsType(Enums.PaymentMethodType type)
        => Ok(await Mediator.Send(new GetCompanyPaymentMethodsQuery()));

    [HttpGet("GetIpgRedirectionMethodType")]
    [EnumDataType(typeof(Enums.IpgRedirectionMethodType))]
    public async Task<IActionResult> GetIpgRedirectionMethodType(Enums.IpgRedirectionMethodType type)
       => Ok(await Mediator.Send(new GetIpgRedirectionMethodTypeQuery()));

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPost]
    public async Task<Result<long>> CreateCompany([FromForm] CreateCompanyModel model)
    {
        CreateCompanyViewModel createCompanyViewModel = new(model.PersianName, model.EnglishName, model.NationalCodeMatchingRequired,
            new FormFileProxy(model.File), model.MethodTypes.ToArray(), model.Users, model.SiteAddress, model.IpgRedirectionMethodType,
            model.ShaparakSetting?.Key, model.ShaparakSetting?.Iv, model.ShaparakSetting?.ThirdPartyCode, model.Code);

        return await Mediator.Send(new CreateCompanyCommand(createCompanyViewModel));
    }

    [Authorize(Policy = AuthPolicies.Roles.Admin)]
    [HttpPut]
    public async Task<Result<Unit>> UpdateCompany([FromForm] UpdateCompanyModel model)
    {
        UpdateCompanyViewModel updateCompanyViewModel = new(model.Id, model.PersianName, model.EnglishName, model.NationalCodeMatchingRequired,
            new FormFileProxy(model.File), model.MethodTypes.ToArray(), model.Users, model.SiteAddress, model.IpgRedirectionMethodType,
            model.ShaparakSetting.Key, model.ShaparakSetting.Iv, model.ShaparakSetting.ThirdPartyCode, model.Code);

        return await Mediator.Send(new UpdateCompanyCommand(updateCompanyViewModel));
    }
}