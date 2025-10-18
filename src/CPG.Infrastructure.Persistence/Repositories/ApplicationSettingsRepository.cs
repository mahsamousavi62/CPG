using CPG.Domain.SharedKernel.ApplicationSettingsAggregate;
using CPG.Domain.SharedKernel;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.DbContexts;
using CPG.Domain.SharedKernel.Logging;
using CPG.Infrastructure.Persistence.Redis;
using CPG.Domain.SharedKernel.Logging;
using Microsoft.EntityFrameworkCore;
using CPG.Domain.SharedKernel.Logging;
using System;
using CPG.Domain.SharedKernel.Logging;
using System.Threading.Tasks;
using CPG.Domain.SharedKernel.Logging;
using CPG.Domain.SharedKernel.Logging;

namespace CPG.Infrastructure.Persistence.Repositories
{
    public class ApplicationSettingsRepository : IApplicationSettingsRepository
    {
        private readonly IRedisCacheService _cacheService;
        private readonly ILogService _logService;
        private readonly ReadDbContext _context;
        public const string CacheKey = "AuthenticationConfigApplicationSettings_key";

        public ApplicationSettingsRepository(ReadDbContext context, IRedisCacheService cacheService,
            ILogService logService)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logService;
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
                }
            }
            catch (Exception ex)
            {
                _logService.LogError($"convert Exception in appsettings:{ex.Message}");
            }

            _cacheService.SetData(CacheKey, cacheData);

            return cacheData;
        }
    }
}
