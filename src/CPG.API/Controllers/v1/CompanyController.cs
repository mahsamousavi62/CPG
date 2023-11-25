using CPG.API.Model;
using CPG.Application.UseCases.Books.Queries;
using CPG.Application.UseCases.Books.ViewModels;
using CPG.Application.UseCases.Common.Queries;
using CPG.Application.UseCases.Common.ViewModels;
using CPG.Application.UseCases.Companies.Commands.Create;
using CPG.Application.UseCases.Companies.Queries;
using CPG.Application.UseCases.Companies.ViewModels;
using CPG.Application.UseCases.Files.Commands.DownloadFile;
using CPG.Application.UseCases.Files.Commands.UploadFile;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.File;
using CPG.Infrastructure.Persistence.GraphQL.Queries;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
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
         => Ok(await Mediator.Send(new GetAllCompanyQuery()));


        [AllowAnonymous]
        [HttpGet("{id:long}")]
        public async Task<ActionResult<CompanyViewModel>> GetCompany(long id)
            => Ok(await Mediator.Send(new GetCompanyQuery(id)));


        [HttpGet("GetCompanyPaymentMethodsType")]
        public async Task<IActionResult> GetCompanyPaymentMethodsType()
            => Ok(await Mediator.Send(new GetCompanyPaymentMethodsQuery()));


        [HttpPost("CreateCompany")]
        public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyModel model, IFormFile file)
        {
            CreateCompanyViewModel createCompanyViewModel = new(
                 model.PersianName, model.EnglishName, model.NationalCodeMatchingRequied,
                 new FormFileProxy(file), model.MethodTypes, model.Users);

            var companyId = await Mediator.Send(new CreateCompanyCommand(createCompanyViewModel));

            return CreatedAtAction(nameof(CreateCompany), new { id = companyId }, new { companyId });


        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
              => Ok(await Mediator.Send(new UploadPhotoCommand(new FormFileProxy(file))));


        [HttpPost("Download")]
        public async Task<IActionResult> Download(string fileName)
           => Ok(await Mediator.Send(new DownloadFileCommand(fileName)));
    }
}
