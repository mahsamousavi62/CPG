using System;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class PaymentRequestMethodDepositReadModel
{
    public long Id { get; set; }

    public long PaymentRequestMethodId { get; set; }

    public long CompanyDepositId { get; set; }

    public CompanyDepositReadModel CompanyDeposit { get; set; }

    public PaymentRequestMethodReadModel PaymentRequestMethod { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }
}