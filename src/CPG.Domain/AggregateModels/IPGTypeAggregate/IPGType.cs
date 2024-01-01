using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;

namespace CPG.Domain.AggregateModels.IPGTypeAggregate;

public class IPGType : AuditableEntity<long>, IAggregateRoot
{
    public IPGType()
    {

    }

    public IPGType(PersianName persianName, EnglishName englishName, Logo logo)
    {
        PersianName = persianName.Value;
        EnglishName = englishName.Value;
        Logo = logo.Value;
    }

    public string PersianName { get; }

    public string EnglishName { get; }

    public string Logo { get; }

    public static IPGType Create(PersianName persianName, EnglishName englishName, Logo logo)
    {
        IPGType provider = new(persianName, englishName, logo)
        {
            IsActive = true
        };
        return provider;
    }
}
