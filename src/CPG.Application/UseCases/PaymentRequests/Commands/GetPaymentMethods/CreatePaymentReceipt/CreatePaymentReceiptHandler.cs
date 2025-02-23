using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyDepositAggregate;
using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreatePaymentReceipt;

public class CreatePaymentReceiptHandler : GetPaymentMethodsHandler
{
    private List<CompanyDeposit> AvailablePaymentReceipt(RequestContext request)
    {
        var validator = new CreatePaymentReceiptValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return null;
        }

        var company = request.Company;
        var paymentRequest = request.PaymentRequest;
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods
            .FirstOrDefault(p => p.PaymentMethodType == PaymentMethodType.PaymentReceipt);

        var paymentRequestMethodDeposits = paymentRequestMethod?.PaymentRequestMethodDeposits ?? new List<PaymentRequestMethodDeposit>();
        var hasDeposits = paymentRequestMethodDeposits.Any();

        var paymentReceiptDeposits = company.CompanyDeposits
            .Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.PaymentReceipt))
            .ToList();

        if (hasDeposits)
        {
            return paymentReceiptDeposits
                .Where(a => paymentRequestMethodDeposits.Select(x => x.Id).Contains(a.Id))
                .ToList();
        }

        return paymentReceiptDeposits;
    }
    public override async Task HandleRequset(PaymentMethodType methodType, RequestContext request, PaymentMethodsViewModel model)
    {
        if (methodType == PaymentMethodType.PaymentReceipt)
        {
            var componyDeposits = AvailablePaymentReceipt(request);
            model.Receipt = new();
            if (componyDeposits is null)
            {
                model.Receipt = null;
            }
            else if (componyDeposits != null)
            {
                model.Receipt.Add(new Receipt
                {
                    AccountNumber = string.Empty,
                    BankName = string.Empty,

                });

            }
        }
        else if (handler != null)
        {
            await handler.HandleRequset(methodType, request, model);
        }
    }
}