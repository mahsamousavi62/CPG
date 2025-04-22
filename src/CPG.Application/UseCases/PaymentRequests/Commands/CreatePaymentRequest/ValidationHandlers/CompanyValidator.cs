using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Domain.AggregateModels.CompanyAggregate;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.ValidationHandlers
{
    internal class CompanyValidator<T> : ValidatorHandler<T>
        where T : Company
    {
        public override void Handle(T company)
        {
            if (company is null)
                throw new PaymentRequestNoCompanyFoundException();

            if (!company.IsActive)
                throw new PamentRequestInactiveCompanyException();

        }
    }
}
