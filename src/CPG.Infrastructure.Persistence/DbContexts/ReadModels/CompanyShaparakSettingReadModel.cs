using System;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class CompanyShaparakSettingReadModel
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string Key { get; set; }
    public string Iv { get; set; }
    public int? ThirdPartyCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? ModificationDate { get; set; }
    public CompanyReadModel Company { get; set; }
}
