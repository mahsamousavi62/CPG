using System;
using System.Collections.Generic;
using static CPG.Domain.SharedKernel.Enums;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class PaymentRequestMethodReadModel
{
    public long Id { get; set; }

    public long PaymentRequestId { get; set; }

    public PaymentMethodType PaymentMethodType { get; set; }

    public PaymentRequestReadModel PaymentRequest { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }

    public List<PaymentRequestMethodDepositReadModel> PaymentRequestMethodDeposits { get; set; }

    public List<PaymentRequestMethodIpgTypeReadModel> PaymentRequestMethodIpgTypes { get; set; }
}