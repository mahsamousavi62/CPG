using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using System;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Domain.AggregateModels.PaymentRequestAggregate;

public class PaymentRequestMethod : AuditableEntity<long>
{
    public long PaymentRequestId { get; set; }

    public Enums.PaymentMethodType PaymentMethodType { get; set; }

    public PaymentRequest PaymentRequest { get; set; }

    public List<PaymentRequestMethodDeposit> PaymentRequestMethodDeposits { get; set; }

    public List<PaymentRequestMethodIpgType> PaymentRequestMethodIpgTypes { get; set; }

    public PaymentRequestMethod()
    {

    }

    public PaymentRequestMethod(Enums.PaymentMethodType methodType, long paymentRequestId)
    {
        PaymentMethodType = methodType;
        PaymentRequestId = paymentRequestId;
        IsActive = true;
    }

    public PaymentRequestMethod(Enums.PaymentMethodType methodType, List<PaymentRequestMethodIpgType> ipgTypes, List<PaymentRequestMethodDeposit> deposits)
    {
        PaymentMethodType = methodType;
        if (deposits != null)
            PaymentRequestMethodDeposits = deposits;
        if (ipgTypes != null)
            PaymentRequestMethodIpgTypes = ipgTypes;
        IsActive = true;
    }

    public static PaymentRequestMethod Create(PaymentMethodType methodType, long[] ipgTypeIds = null, long[] companyDepositIds = null)
    {
        List<PaymentRequestMethodIpgType> ipgTypes = null;
        if (methodType == PaymentMethodType.InternetPaymentGateway && ipgTypeIds?.Any() is true)
        {
            ipgTypes = PaymentRequestMethodIpgType.Create(ipgTypeIds);
        }
        List<PaymentRequestMethodDeposit> deposits = null;
        if (companyDepositIds?.Any() is true)
        {
            deposits = PaymentRequestMethodDeposit.Create(companyDepositIds);
        }

        return new PaymentRequestMethod(methodType, ipgTypes, deposits);        
    }
}