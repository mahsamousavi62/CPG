using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Application.UseCases.DirectDebit.Query;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Application.UseCases.Ipg.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CPG.API.Controllers.v1
{
    [Authorize]
    public class DirectDebitController : ApiBaseController
    {
        [HttpPost("GetDirectDebitToken")]
        [ProducesResponseType(typeof(Result<bool>), 200)]
        public async Task<Result<bool>> GetToken([FromBody] CreateDirectDebitRequestViewModel request)
            => await Mediator.Send(new CreateDirectDebitRequestCommand(request));

        [HttpGet("GetAvailableBankList")]
        [ProducesResponseType(typeof(Result<IReadOnlyCollection<AvailableBankViewModel>>), 200)]
        public async Task<Result<IReadOnlyCollection<AvailableBankViewModel>>> GetAvailableBankList()
        {
            return await Mediator.Send(new GetAvailableBankListQuery());
        }

        [HttpPost("GetPlans")]
        [ProducesResponseType(typeof(Result<PlanViewModel>), 200)]
        public async Task<Result<PlanViewModel>> GetPlans([FromBody] GetDirectDebitPlansViewModel request)
            => await Mediator.Send(new GetDirectDebitPlansCommand(request));

        [HttpPost("GetUserPhoneNumbers")]
        [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserPhoneNumberViewModel>>), 200)]
        public async Task<Result<IReadOnlyCollection<UserPhoneNumberViewModel>>> GetUserPhoneNumbers([FromBody] GetUserPhoneNumbersViewModel request)
           => await Mediator.Send(new GetUserPhoneNumbersCommand(request));

        [HttpPost("ConfirmGrant")]
        [ProducesResponseType(typeof(Result<ConfirmGrantResponseViewModel>), (int)HttpStatusCode.OK)]
        public async Task<Result<ConfirmGrantResponseViewModel>> ConfirmGrant([FromBody] ConfirmGrantViewModel request)
        {
            var response = await Mediator.Send(new ConfirmGrantCommand(request));
            return response;
        }
    }
}
