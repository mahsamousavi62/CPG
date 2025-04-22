using CPG.Domain.AggregateModels.PaymentRequestAggregate;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System.Collections.Generic;

namespace CPG.Domain.AggregateModels.IPGTypeAggregate;

public class IPGType : AuditableEntity<long>, IAggregateRoot
{
    public IPGType()
    {

    }

    public IPGType(PersianName persianName, EnglishName englishName, Logo logo, short code)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        Logo = logo.Value;
        Code = code;
    }

    public string PersianName { get; }

    public string EnglishName { get; }

    public string Logo { get; }

    public short Code { get; set; }

    public List<PaymentRequestMethodIpgType> PaymentRequestMethodIpgTypes { get; set; }

    public static IPGType Create(PersianName persianName, EnglishName englishName, Logo logo, short code)
    {
        IPGType provider = new(persianName, englishName, logo, code)
        {
            IsActive = true
        };
        return provider;
    }
}