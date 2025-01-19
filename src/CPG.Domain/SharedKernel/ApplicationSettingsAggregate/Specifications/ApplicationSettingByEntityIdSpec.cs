using Ardalis.Specification;
using System.Linq;

namespace CPG.Domain.SharedKernel.ApplicationSettingsAggregate.Specifications;

public sealed class ApplicationSettingByEntityTypeSpec : Specification<ApplicationSettings>
{
    public ApplicationSettingByEntityTypeSpec(Enums.ApplicationSettingEntityType entityType)
    {
        Query
            .Where(app => app.EntityType == entityType);
    }
}
