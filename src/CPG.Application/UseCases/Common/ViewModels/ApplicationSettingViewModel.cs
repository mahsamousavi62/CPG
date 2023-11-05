using CPG.Domain.SharedKernel;

namespace CPG.Application.UseCases.Common.ViewModels;

public class ApplicationSettingViewModel
{
    public long Id { get; set; }

    public Enums.ApplicationSettingEntityType EntityType { get; set; }

    public string EntityTypeName { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
