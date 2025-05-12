using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class ActiveMethodValidatorModel
{
    public List<PaymentMethodType> ActiveMethods { get; set; }
    public Company Company { get; set; }
}
