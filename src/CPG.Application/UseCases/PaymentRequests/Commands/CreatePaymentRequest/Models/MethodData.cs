using CPG.Domain.AggregateModels.IPGTypeAggregate;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Application.UseCases.PaymentRequests.Commands.CreatePaymentRequest.Models;

internal class MethodData
{
    public PaymentMethodType MethodType { get; set; }
    public List<IbanInfo> IbanInfoList { get; set; }
    public List<IPGType> IPGTypeList { get; set; }
}
