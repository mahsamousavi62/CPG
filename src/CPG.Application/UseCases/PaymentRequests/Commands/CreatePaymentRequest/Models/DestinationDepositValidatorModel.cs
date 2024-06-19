using CPG.Application.UseCases.PaymentRequests.ViewModels;
using CPG.Domain.AggregateModels.CompanyAggregate;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class DestinationDepositValidatorModel
{
    public List<string> DestinationDeposits { get; set; }
    public Company Company { get; set; }
    public PaymentMethodConfig PaymentMethodConfig { get; set; }
    public List<PaymentMethodType> ActiveMethods { get; set; }
}
