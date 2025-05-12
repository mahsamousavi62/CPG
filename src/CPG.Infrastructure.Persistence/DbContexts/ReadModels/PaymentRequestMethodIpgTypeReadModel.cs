using System;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class PaymentRequestMethodIpgTypeReadModel
{
    public long Id { get; set; }

    public long PaymentRequestMethodId { get; set; }

    public short IpgTypeId { get; set; }

    public IPGTypeReadModel IpgType { get; set; }

    public PaymentRequestMethodReadModel PaymentRequestMethod { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ModificationDate { get; set; }
}