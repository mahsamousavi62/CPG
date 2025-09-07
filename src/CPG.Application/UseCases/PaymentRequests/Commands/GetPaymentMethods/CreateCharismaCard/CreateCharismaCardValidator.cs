using FluentValidation;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateCharismaCard;

public class CreateCharismaCardValidator : AbstractValidator<RequestContext>
{
    public CreateCharismaCardValidator()
    {

        RuleFor(request => request.PaymentRequest.PaymentRequestMethods)
.Must(methods => methods?.Any(p => p.PaymentMethodType == PaymentMethodType.CharismaCard) == true)
.When(request => request.PaymentRequest.PaymentRequestMethods.Count != 0);
        RuleFor(request => request.Company.PaymentMethods)
            .Must(methods => methods.Any(x => x.MethodType == PaymentMethodType.CharismaCard));

        RuleFor(request => request.Company.CompanyDeposits).NotEmpty();

        RuleFor(request => request.Company.CompanyDeposits)
            .Must(deposits => deposits.Any(d => d.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.CharismaCard)));

        RuleFor(request => request.Company.CompanyDeposits)
            .Must(deposits =>
            {
                var defaultDeposit = deposits.FirstOrDefault(d => d.IsDefaultForCharismaCard == true && d.IsActive);
                return defaultDeposit != null && defaultDeposit.Bank.IsActive;
            });
    }
}
