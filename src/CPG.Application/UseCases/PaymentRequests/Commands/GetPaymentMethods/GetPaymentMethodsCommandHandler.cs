using CPG.Application.Shared.Resource;
using CPG.Application.UseCases.Exceptions;
using CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest;
using CPG.Application.UseCases.PaymentRequests.Exceptions;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using CPG.Domain.AggregateModels.CompanyAggregate.Specifications;
using CPG.Domain.AggregateModels.PaymentRequestAggregate.Specifications;
using CPG.Domain.SharedKernel;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods;

public class GetPaymentMethodsCommandHandler(IAggregateRepository<PaymentRequest> paymentRequestRepository,
    IAggregateRepository<Company> companyRepository) : IRequestHandler<GetPaymentMethodsCommand, PaymentMethodsViewModel>
{
    private readonly IAggregateRepository<PaymentRequest> _paymentRequestRepository = paymentRequestRepository;
    private readonly IAggregateRepository<Company> _companyRepository = companyRepository;

    public async Task<PaymentMethodsViewModel> Handle(GetPaymentMethodsCommand request, CancellationToken cancellationToken)
    {
        var paymentRequest = await _paymentRequestRepository.FirstOrDefaultAsync(new PaymentRequestByCode(request.ViewModel.Code));

        if (paymentRequest is null)
        {
            throw new PaymentRequestCodeNotFoundException();
        }

        if (paymentRequest.UrlExpirationDateTime < DateTime.UtcNow)
        {
            throw new PaymentRequestCodeIsExpiredException();
        }

        if (paymentRequest.IsUsed)
        {
            throw new PaymentRequestCodeIsUsedException();
        }

        if (paymentRequest.IsVerified)
        {
            throw new PaymentRequestCodeIsFinalizedException();
        }

        if (paymentRequest.Status != 0)
        {
            throw new PaymentRequestStatusIsInvalidException();
        }

        paymentRequest.Status = 1;
        await _paymentRequestRepository.UpdateAsync(paymentRequest);

        Company company = null;
        if (!string.IsNullOrEmpty(paymentRequest.DestinationIban))
        {
            company = await _companyRepository.GetBySpecAsync(new CompanyByIdAndDepositIbanSpec(paymentRequest.CompanyId, paymentRequest.DestinationIban), cancellationToken);
        }
        else
        {
            company = await _companyRepository.GetBySpecAsync(new CompanyFullDataByIdSpec(paymentRequest.CompanyId), cancellationToken);
        }

        if (!company.IsActive)
        {
            throw new CompanyIsInactiveException(company.PersianName);
        }

        return new PaymentMethodsViewModel
        {
            Amount = paymentRequest.Amount,
            //IPGs = company.
        };
    }
}
