using static CPG.Domain.SharedKernel.Enums;
using FluentValidation;
using System.Linq;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg;

public class CreateIpgValidator : AbstractValidator<RequestContext>
{
    public CreateIpgValidator()
    {
        RuleFor(request => request.PaymentRequest.PaymentRequestMethods)
              .Must(methods => methods.Any(p => p.PaymentMethodType == PaymentMethodType.InternetPaymentGateway));

        RuleFor(request => request.Company.PaymentMethods)
            .Must(methods => methods.Any(x => x.MethodType == PaymentMethodType.InternetPaymentGateway));

        RuleFor(request => request.Company.CompanyDeposits).NotEmpty();

        RuleFor(request => request.Company.CompanyDeposits)
            .Must(deposits => deposits.Any(d => d.PaymentMethods.Any(x => x.MethodType == PaymentMethodType.InternetPaymentGateway)));
    }
}
