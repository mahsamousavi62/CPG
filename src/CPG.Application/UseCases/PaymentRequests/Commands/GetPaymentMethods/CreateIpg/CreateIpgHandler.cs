
using CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg.AvailableIpgStrategy;
using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyIPGAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.GetPaymentMethods.CreateIpg;

public class CreateIpgHandler : GetPaymentMethodsHandler
{
    public List<CompanyIPG> AvailableIpg(RequestContext request)
    {
        var company = request.Company;
        var paymentRequest = request.PaymentRequest;

        var validator = new CreateIpgValidator();

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return null;
        }
        var paymentRequestMethod = paymentRequest.PaymentRequestMethods.FirstOrDefault
                                   (p => p.PaymentMethodType == PaymentMethodType.InternetPaymentGateway);

        var IpgCompanyDeposits = company.CompanyDeposits.
            Where(t => t.PaymentMethods.Select(x => x.MethodType).Contains(PaymentMethodType.InternetPaymentGateway)).ToList();

        var companyIPGs = company.CompanyIPGs.Where(t => t.Provider.PaymentMethods != null &&
                                                    t.Provider.PaymentMethods.Select(x => x.MethodType)
                                                    .Contains(PaymentMethodType.InternetPaymentGateway) &&
                                                    t.IPGDeposits.Any(c => IpgCompanyDeposits.Select(a => a.Id)
                                                    .Contains(c.CompanyDepositId))).ToList();

        if (!companyIPGs.Any())
        {
            return null;
        }

        IAvailableIpgStrategy strategy;
        var paymentRequestMethodIpgTypes = paymentRequestMethod.PaymentRequestMethodIpgTypes;
        var paymentRequestMethodDeposits = paymentRequestMethod.PaymentRequestMethodDeposits;
        var hasIpgTypes = paymentRequestMethodIpgTypes.Any();
        var hasDeposits = paymentRequestMethodDeposits.Any();
        if (!hasIpgTypes && !hasDeposits)
        {
            strategy = new NoIpgTypeNoDepositStrategy();
        }
        else if (!hasIpgTypes && hasDeposits)
        {
            strategy = new NoIpgTypeWithDepositStrategy();
        }
        else if (hasIpgTypes && !hasDeposits)
        {
            strategy = new WithIpgTypeNoDepositStrategy();
        }
        else
        {
            strategy = new WithIpgTypeWithDepositStrategy();
        }
        return strategy.GetAvailableIpg(company, paymentRequestMethodIpgTypes, paymentRequestMethodDeposits, companyIPGs);
    }

    public override async Task HandleRequset(PaymentMethodType methodType,
        RequestContext request, PaymentMethodsViewModel model)
    {
        if (methodType == PaymentMethodType.InternetPaymentGateway)
        {
            var companyIpgs = AvailableIpg(request);
            if (companyIpgs != null)
            {
                model.IPGs = new();

                var ipgs = await Task.WhenAll(companyIpgs?.Select(t => new { t.IPGType, t.Id }).Select(async t => new IPGInfo
                {
                    Id = t.Id,
                    Logo = await request.MinioProvider.PresignedGetObject(t.IPGType.Logo),
                    PersianName = t.IPGType.PersianName,
                })).ConfigureAwait(false);
                model.IPGs = ipgs.ToList();
            }
        }
        else if (handler != null)
        {
            await handler.HandleRequset(methodType, request, model);
        }
    }
}
