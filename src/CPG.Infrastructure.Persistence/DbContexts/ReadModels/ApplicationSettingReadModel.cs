using CPG.Domain.SharedKernel;

namespace CPG.Infrastructure.Persistence.DbContexts.ReadModels;

public class ApplicationSettingReadModel
{
    public long Id { get; set; }

    public Enums.ApplicationSettingEntityType EntityType { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}
