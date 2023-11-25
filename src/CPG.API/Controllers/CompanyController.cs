
using CPG.Application.UseCases.Books.Queries;
using CPG.Application.UseCases.Books.ViewModels;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Application.UseCases.Companies.Commands.Create;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.GraphQL.Queries;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers
{

    public class CompanyController : ApiBaseController
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyCollection<ApplicationSettingViewModel>>> GetAll()
         => Ok(await Mediator.Send(new GetAllCompaniesQuery()));


        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CompanyViewModel>> GetCompany(long id)
            => Ok(await Mediator.Send(new GetCompanyQuery(id)));


        [HttpGet("GetCompanyPaymentMethods")]
        public async Task<IActionResult> GetCompanyPaymentMethods()
            => Ok(await Mediator.Send(new GetCompanyPaymentMethodsQuery()));


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateCompany(CreateCompanyViewModel model)
        => Ok(await Mediator.Send(new CreateCompanyCommand(model)));


    }
}
