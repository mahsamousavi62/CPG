using CPG.Domain.AggregateModels.CompanyAggregate.Exceptions;
using CPG.Domain.SeedWork;
using CPG.Domain.SharedKernel;
using System.Text.RegularExpressions;

namespace CPG.Domain.AggregateModels.CompanyAggregate;

public class CompanyShaparakSetting : AuditableEntity<long>
{
    public long CompanyId { get; set; }
    public string Key { get; set; }
    public string Iv { get; set; }
    public int? ThirdPartyCode { get; set; }
    public Company Company { get; set; }

    public CompanyShaparakSetting(string key, string iv, int? thirdPartyCode, long companyId)
    {
        CompanyId = companyId;
        Key = key;
        Iv = iv;
        ThirdPartyCode = thirdPartyCode;
        IsActive = true;
    }

    public CompanyShaparakSetting(string key, string iv, int? thirdPartyCode)
    {
        Key = key;
        Iv = iv;
        ThirdPartyCode = thirdPartyCode;
        IsActive = true;
    }

    public static CompanyShaparakSetting Create(string key, string iv, int? thirdPartyCode)
    {
        if (key.Length < 3 || key.Length > 255)
            throw new InvalidKeyCharachterException(key);

        if (iv.Length < 3 || iv.Length > 255)
            throw new InvalidIvCharachterException(iv);

        if (!Regex.IsMatch(key, "[A-Za-z\\d\\s]+"))
            throw new InvalidKeyFormatException(key);

        if (!Regex.IsMatch(iv, "[A-Za-z\\d\\s]+"))
            throw new InvalidIVFormatException(iv);

        var setting = new CompanyShaparakSetting(key, iv, thirdPartyCode);        
        return setting;
    }

    
}