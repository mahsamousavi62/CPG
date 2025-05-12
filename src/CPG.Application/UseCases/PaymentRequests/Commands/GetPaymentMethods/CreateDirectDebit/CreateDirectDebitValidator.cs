using FluentValidation;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateDirectDebit;

public class CreateDirectDebitValidator : AbstractValidator<RequestContext>
{
    public CreateDirectDebitValidator()
    {
        RuleFor(request => request.PaymentRequest.PaymentRequestMethods)
.Must(methods => methods?.Any(p => p.PaymentMethodType == PaymentMethodType.DirectDebit) == true)
.When(request => request.PaymentRequest.PaymentRequestMethods.Count != 0);

        RuleFor(request => request.Company.PaymentMethods)
            .Must(methods => methods.Any(x => x.MethodType == PaymentMethodType.DirectDebit));

        RuleFor(request => request.Company.CompanyDeposits).NotEmpty();

        RuleFor(request => request.Company.CompanyDeposits)
            .Must(deposits => deposits.Any(d => d.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.DirectDebit)));
    }
}
