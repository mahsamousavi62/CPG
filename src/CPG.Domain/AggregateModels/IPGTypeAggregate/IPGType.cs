using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System;

namespace CPG.Domain.AggregateModels.IPGTypeAggregate;

public class IPGType : AuditableEntity<long>, IAggregateRoot
{
    public IPGType()
    {

    }
    public IPGType(PersianName persianName, EnglishName englishName, Logo logo)
    {
        _persianName = persianName.Value;
        _englishName = englishName.Value;
        _logo = logo.Value;
    }

    private string _persianName;
    private string _englishName;
    private string _logo;
    public string PersianName => _persianName;
    public string EnglishName => _englishName;
    public string Logo => _logo;

    public static IPGType Create(PersianName persianName, EnglishName englishName, Logo logo)
    {
        var provider = new IPGType(persianName, englishName, logo);
        provider.IsActive = true;
        return provider;
    }
}
