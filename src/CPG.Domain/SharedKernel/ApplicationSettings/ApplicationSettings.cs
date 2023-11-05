using Ardalis.GuardClauses;
using CPG.Domain.SeedWork;

namespace CPG.Domain.SharedKernel.ApplicationSettings
{
    public class ApplicationSettings : AuditableEntity<long>, IAggregateRoot
    {
        public ApplicationSettings(long id, Enums.ApplicationSettingEntityType entityType, string key, string value)
        {
            Key = Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Value = Guard.Against.NullOrWhiteSpace(value, nameof(value));
            EntityType = entityType;
            Id = Guard.Against.NegativeOrZero(id, nameof(id));
        }

        public Enums.ApplicationSettingEntityType EntityType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }


        public static ApplicationSettings Update(int id, Enums.ApplicationSettingEntityType entityType, string key, string value)
        {
            var applicationSetting = new ApplicationSettings(id, entityType, key, value);
            return applicationSetting;
        }
    }
}
