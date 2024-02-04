using CPG.Application.UseCases.Banks.ViewModels;
using CPG.Application.UseCases.DirectDebit.Commands;
using CPG.Application.UseCases.DirectDebit.Query;
using CPG.Application.UseCases.DirectDebit.ViewModels;
using CPG.Domain.SharedKernel;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CPG.API.Controllers.v1
{
    public class DirectDebitController : ApiBaseController
    {
        [HttpPost("GetDirectDebitToken")]
        [Authorize]
        [ProducesResponseType(typeof(Result<bool>), 200)]
        public async Task<Result<bool>> GetToken([FromBody] CreateDirectDebitRequestViewModel request)
            => await Mediator.Send(new CreateDirectDebitRequestCommand(request));

        [HttpGet("GetAvailableBankList")]
        [Authorize]
        [ProducesResponseType(typeof(Result<IReadOnlyCollection<BankViewModel>>), 200)]
        public async Task<Result<IReadOnlyCollection<BankViewModel>>> GetAvailableBankList()
        {
            return await Mediator.Send(new GetAvailableBankListQuery());
        }

        [HttpPost("GetDirectDebitPlans")]
        [Authorize]
        [ProducesResponseType(typeof(Result<PlanViewModel>), 200)]
        public async Task<Result<PlanViewModel>> GetDirectDebitPlans([FromBody] GetDirectDebitPlansViewModel request)
            => await Mediator.Send(new GetDirectDebitPlansCommand(request));
    }
}
