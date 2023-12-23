using System.Threading.Tasks;

namespace CPG.Domain.SharedKernel.ApplicationSettings;

public interface IApplicationSettingsRepository
{
    Task<ApplicationConfigViewModel> GetAllApplicationSettings();
}
