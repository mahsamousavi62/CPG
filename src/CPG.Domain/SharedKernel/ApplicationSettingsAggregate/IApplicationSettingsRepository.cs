using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ApplicationSettingsAggregate;

public interface IApplicationSettingsRepository
{
    Task<ApplicationConfigViewModel> GetAllApplicationSettings();
}
