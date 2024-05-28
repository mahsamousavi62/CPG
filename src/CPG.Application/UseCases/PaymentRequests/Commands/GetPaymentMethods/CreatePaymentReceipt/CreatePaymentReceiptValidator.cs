using static CPG.Domain.SharedKernel.Enums;
using FluentValidation;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreatePaymentReceipt;

public class CreatePaymentReceiptValidator : AbstractValidator<RequestContext>
{
    public CreatePaymentReceiptValidator()
    {
        RuleFor(request => request.PaymentRequest.PaymentRequestMethods)
            .Must(methods => methods.Any(p => p.PaymentMethodType == PaymentMethodType.PaymentReceipt));

        RuleFor(request => request.Company.PaymentMethods)
            .Must(methods => methods.All(x => x.MethodType != PaymentMethodType.PaymentReceipt));

        RuleFor(request => request.Company.CompanyDeposits)
            .NotEmpty();

        RuleFor(request => request.Company.CompanyDeposits)
            .Must(deposits => deposits.Any(d => d.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.PaymentReceipt)));
    }
}
