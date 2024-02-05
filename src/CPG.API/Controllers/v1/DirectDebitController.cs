using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Application.UseCases.DirectDebit.Query;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(Result<IReadOnlyCollection<BankViewModel>>), 200)]
        public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetAvailableBankList()
        {
            return await Mediator.Send(new GetAvailableBankListQuery());
        }

        [HttpPost("GetDirectDebitPlans")]
        [ProducesResponseType(typeof(Result<PlanViewModel>), 200)]
        public async Task<Result<PlanViewModel>> GetDirectDebitPlans([FromBody] GetDirectDebitPlansViewModel request)
            => await Mediator.Send(new GetDirectDebitPlansCommand(request));

        [HttpPost("GetUserPhoneNumbers")]
        [ProducesResponseType(typeof(Result<IReadOnlyCollection<UserPhoneNumberViewModel>>), 200)]
        public async Task<Result<IReadOnlyCollection<UserPhoneNumberViewModel>>> GetUserPhoneNumbers([FromBody] GetUserPhoneNumbersViewModel request)
           => await Mediator.Send(new GetUserPhoneNumbersCommand(request));

        [HttpPost("SetUserDirectDebitPlan")]
        [ProducesResponseType(typeof(Result<bool>), 200)]
        public async Task<Result<bool>> SetUserDirectDebitPlan([FromBody] SetUserDirectDebitPlanViewModel request)
            => await Mediator.Send(new SetUserDirectDebitPlanCommand(request));
    }
}
