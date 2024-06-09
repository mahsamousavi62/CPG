using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class DepositExistInIpgValidatorModel
{
    public Company Company { get; set; }
    public List<MethodData> MethodData { get; set; }
    public PaymentMethodConfig PaymentMethodConfig { get; set; }
}
