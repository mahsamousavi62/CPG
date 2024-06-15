using CPG.Domain.AggregateModels.BankAggregate;
using CPG.Domain.AggregateModels.CompanyDepositAggregate.Exceptions;
using FluentValidation;

namespace CPG.Application.UseCases.PaymentRequests.Queries.Report;

public class PaymentFilterValidator : AbstractValidator<PaymentFilter>
{
    public PaymentFilterValidator()
    {

     RuleFor(x => x.DestinationDepositIban).Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(GlobalResource.InvalidIbanFormat)
            .Length(26).WithMessage(GlobalResource.InvalidIbanFormat)
            .Must(iban =>
            {
                if (!iban[..2].ToCharArray().All(t => char.IsLetter(t)) || iban[..2] != "IR")
                    return false;

                return true;
            }).WithMessage(GlobalResource.InvalidIbanFormat)
            .When(x => !string.IsNullOrEmpty(x.DestinationDepositIban));

        RuleFor(p => p.NationalCode)
            .Matches("^[0-9]{10}$").WithMessage(GlobalResource.InvalidNationalCode);

        RuleFor(p => p.Amount)
            .GreaterThan(10000).When(p => p.Amount.HasValue).WithMessage(GlobalResource.AmountIsNotInRange);
        RuleFor(p => p.Amount).
         LessThan(100000000000m).When(p => p.Amount.HasValue).WithMessage(GlobalResource.AmountIsNotInRange);

        RuleFor(p => p.Status)
            .IsInEnum().WithMessage(GlobalResource.PaymentRequestStatusIsInvalid);

        RuleFor(p => p.FromUrlExpirationDateTime)
            .LessThanOrEqualTo(p => p.ToUrlExpirationDateTime)
            .When(p => p.FromUrlExpirationDateTime.HasValue && p.ToUrlExpirationDateTime.HasValue)
            .WithMessage(GlobalResource.FromDateGreatherThanToDate);

        RuleFor(p => p.FromCreationDate)
            .LessThanOrEqualTo(p => p.ToCreationDate)
            .When(p => p.FromCreationDate.HasValue && p.ToCreationDate.HasValue)
            .WithMessage(GlobalResource.FromDateGreatherThanToDate);

        RuleFor(p => p.FromModificationDate)
            .LessThanOrEqualTo(p => p.ToModificationDate)
            .When(p => p.FromModificationDate.HasValue && p.ToModificationDate.HasValue)
            .WithMessage(GlobalResource.FromDateGreatherThanToDate);
    }
}
