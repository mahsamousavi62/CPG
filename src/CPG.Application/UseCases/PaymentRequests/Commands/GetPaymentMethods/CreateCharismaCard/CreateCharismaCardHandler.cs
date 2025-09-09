using CPG.Application.UseCases.PaymentRequests.ViewModels;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateCharismaCard;

public class CreateCharismaCardHandler : GetPaymentMethodsHandler
{
	private bool AvailableCharismaCard(RequestContext request)
	{
		CreateCharismaCardValidator validator = new CreateCharismaCardValidator();
		FluentValidation.Results.ValidationResult validationResult = validator.Validate(request);

		if (!validationResult.IsValid)
		{
			return false;
		}

		Domain.AggregateModels.CompanyAggregate.Company company = request.Company;
		List<Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit> charismaCardCompanyDeposit = company.CompanyDeposits
			.Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.CharismaCard))
			.ToList();

		Domain.AggregateModels.CompanyDepositAggregate.CompanyDeposit isDefault = charismaCardCompanyDeposit.FirstOrDefault(c => c.IsDefaultForCharismaCard == true && c.IsActive);

		if (!isDefault.Bank.IsActive)
		{
			return false;
		}

		return true;
	}
	public override async Task HandleRequset(PaymentMethodType methodType, RequestContext request, PaymentMethodsViewModel model)
	{
		if (methodType == PaymentMethodType.CharismaCard && AvailableCharismaCard(request))
		{
			var result = await request.charismaCardService.GetUserDepositBalance(request.NationalCode);

			if (result.IsSuccess && result?.Data?.Data is not null)
			{
				model.CharismaCard = result.Data.Data?.Select(account => new CPG.Application.UseCases.PaymentRequests.ViewModels.CharismaCard
				{
					Balance = account.Balance,
					CustomerFirstName = account.CustomerFirstName,
					CustomerLastName = account.CustomerLastName,
					Iban = account.Iban,
					CardNumber = account.CardNumber,
					DepositNumber = account.DepositNumber,
					UrlAliasName = account.UrlAliasName
				}).ToList() ?? [];
			}
		}
		else if (handler != null)
		{
			await handler.HandleRequset(methodType, request, model);
		}
	}
}