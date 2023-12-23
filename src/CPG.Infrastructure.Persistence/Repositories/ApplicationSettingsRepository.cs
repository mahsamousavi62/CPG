using CPG.Domain.SharedKernel.ApplicationSettings;
using CPG.Domain.SharedKernel;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Infrastructure.Persistence.Redis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly IRedisCaheService _cacheService;
        private readonly ILogger<ApplicationSettingsRepository> _logger;
        private readonly ReadDbContext _context;
        public const string CacheKey = "AuthenticationConfigApplicationSettings_key";

        public ApplicationSettingsRepository(ReadDbContext context, IRedisCaheService cacheService,
            ILogger<ApplicationSettingsRepository> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<ApplicationConfigViewModel> GetAllApplicationSettings()
        {
            var cacheData = _cacheService.GetData<ApplicationConfigViewModel>(CacheKey);

            if (cacheData != null)
                return cacheData;

            var appSettings = await _context.ApplicationSettingReadModels.ToDictionaryAsync(t => t.Key, t => t.Value);

            cacheData = new ApplicationConfigViewModel();
            var type = cacheData.GetType();
            var prs = type.GetProperties();
            try
            {
                foreach (var item in prs)
                {
                    item.SetValue(cacheData, Convert.ChangeType(appSettings[item.Name], Type.GetTypeCode(item.PropertyType)));
                    _logger.LogInformation($"appSettings: { appSettings[item.Name]}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"convert Exception in appsettings:{ ex.Message}");
            }

            _cacheService.SetData(CacheKey, cacheData);

            return cacheData;
        }
    }
}
