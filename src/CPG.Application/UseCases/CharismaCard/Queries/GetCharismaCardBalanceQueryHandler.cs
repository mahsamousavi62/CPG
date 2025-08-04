using CPG.Application.UseCases.CharismaCard.ViewModels;
using CPG.Domain.SharedKernel.Communication.CharismaCard;
using System.Collections.Generic;

namespace CPG.Application.UseCases.CharismaCard.Queries;

public class GetCharismaCardBalanceQueryHandler : IRequestHandler<GetCharismaCardBalanceQuery, Result<CharismaCardBalanceViewModel>>
{
    private readonly ICharismaCardService _charismaCardService;

    public GetCharismaCardBalanceQueryHandler(ICharismaCardService charismaCardService)
    {
        _charismaCardService = charismaCardService;
    }

    public async Task<Result<CharismaCardBalanceViewModel>> Handle(GetCharismaCardBalanceQuery request, CancellationToken cancellationToken)
    {
        var result = await _charismaCardService.GetUserDepositBalance("");

        if (result.IsSuccess)
        {
            var accounts = result.Data.Data?.Select(account => new CharismaCardAccountViewModel
            {
                Balance = account.Balance,
                CustomerFirstName = account.CustomerFirstName,
                CustomerLastName = account.CustomerLastName,
                Iban = account.Iban,
                CardNumber = account.CardNumber,
                DepositNumber = account.DepositNumber,
                UrlAliasName = account.UrlAliasName
            }).ToList() ?? new List<CharismaCardAccountViewModel>();

            var viewModel = new CharismaCardBalanceViewModel
            {
                IsSuccess = result.Data.IsSuccess,
                Accounts = accounts,
                ErrorCode = result.Data.Error?.Code,
                ErrorDescription = result.Data.Error?.Description
            };

            return Result<CharismaCardBalanceViewModel>.SuccessResult(viewModel);
        }

        return Result<CharismaCardBalanceViewModel>.Failure(new Error("", ""));
    }
}